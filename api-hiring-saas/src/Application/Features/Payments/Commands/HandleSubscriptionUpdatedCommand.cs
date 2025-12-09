using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Payments;
using Domain.Interfaces;
using DTO.Enums.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Commands;

public sealed record HandleSubscriptionUpdatedCommand(
    string SubscriptionId,
    string CustomerId,
    string Status,
    DateTime? TrialEnd,
    DateTime CurrentPeriodEnd
) : ICommand;

public sealed class HandleSubscriptionUpdatedCommandHandler : ICommandHandler<HandleSubscriptionUpdatedCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;
    private readonly ILogger<HandleSubscriptionUpdatedCommandHandler> _logger;

    public HandleSubscriptionUpdatedCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IDateTime dateTime,
        ILogger<HandleSubscriptionUpdatedCommandHandler> logger)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task Handle(HandleSubscriptionUpdatedCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing subscription updated for SubscriptionId: {SubscriptionId}", command.SubscriptionId);

        // Find user by subscription ID
        var user = await _context.User
            .FirstOrDefaultAsync(u => u.StripeSubscriptionId == command.SubscriptionId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("No user found for SubscriptionId: {SubscriptionId}", command.SubscriptionId);
            return;
        }

        // Update user subscription status
        user.UpdateSubscriptionStatus(command.Status, _dateTime, command.CustomerId, command.SubscriptionId);

        // Handle different subscription statuses
        switch (command.Status)
        {
            case "trialing":
                if (command.TrialEnd.HasValue)
                {
                    user.StartTrial(DateTime.UtcNow, command.TrialEnd.Value, command.CustomerId, command.SubscriptionId);
                }
                break;
                
            case "active":
                user.StartSubscription(DateTime.UtcNow, command.CurrentPeriodEnd);
                break;
                
            case "canceled":
            case "incomplete_expired":
                // Handle cancellation
                break;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated subscription for user {UserId}, Status: {Status}", 
            user.Id, command.Status);
    }
}