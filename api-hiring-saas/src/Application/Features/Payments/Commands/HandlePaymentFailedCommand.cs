using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Payments;
using Domain.Entities.User;
using DTO.Enums.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Commands;

public sealed record HandlePaymentFailedCommand(
    string PaymentIntentId,
    string? CustomerId,
    decimal Amount,
    string Currency,
    string? FailureReason
) : ICommand;

public sealed class HandlePaymentFailedCommandHandler : ICommandHandler<HandlePaymentFailedCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HandlePaymentFailedCommandHandler> _logger;

    public HandlePaymentFailedCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<HandlePaymentFailedCommandHandler> logger)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(HandlePaymentFailedCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment failed for PaymentIntent: {PaymentIntentId}", command.PaymentIntentId);

        // Check if we already have a record for this payment
        var existingTransaction = await _context.PaymentTransaction
            .FirstOrDefaultAsync(t => t.StripePaymentIntentId == command.PaymentIntentId, cancellationToken);

        if (existingTransaction != null)
        {
            // Update existing transaction
            existingTransaction.Status = PaymentStatus.Failed;
            existingTransaction.FailureReason = command.FailureReason;
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        // Find user by Stripe customer ID
        ApplicationUser? user = null;
        if (!string.IsNullOrEmpty(command.CustomerId))
        {
            user = await _context.User
                .FirstOrDefaultAsync(u => u.StripeCustomerId == command.CustomerId, cancellationToken);
        }

        if (user == null)
        {
            _logger.LogWarning("No user found for CustomerId: {CustomerId} and PaymentIntent: {PaymentIntentId}", 
                command.CustomerId, command.PaymentIntentId);
            return;
        }

        // Create new failed payment transaction
        var transaction = new PaymentTransaction
        {
            UserId = user.Id,
            StripePaymentIntentId = command.PaymentIntentId,
            StripeCustomerId = command.CustomerId,
            Amount = command.Amount,
            Currency = command.Currency,
            Status = PaymentStatus.Failed,
            FailureReason = command.FailureReason
        };

        _context.PaymentTransaction.Add(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Recorded failed payment for user {UserId}, PaymentIntent: {PaymentIntentId}, Reason: {FailureReason}", 
            user.Id, command.PaymentIntentId, command.FailureReason);
    }
}