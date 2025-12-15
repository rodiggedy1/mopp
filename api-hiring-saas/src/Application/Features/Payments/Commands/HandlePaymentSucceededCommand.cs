using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Payments;
using Domain.Entities.User;
using Domain.Interfaces;
using DTO.Enums.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Commands;

public sealed record HandlePaymentSucceededCommand(
    string PaymentIntentId,
    string? CustomerId,
    decimal Amount,
    string Currency
) : ICommand;

public sealed class HandlePaymentSucceededCommandHandler : ICommandHandler<HandlePaymentSucceededCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;
    private readonly ILogger<HandlePaymentSucceededCommandHandler> _logger;

    public HandlePaymentSucceededCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IDateTime dateTime,
        ILogger<HandlePaymentSucceededCommandHandler> logger)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task Handle(HandlePaymentSucceededCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment succeeded for PaymentIntent: {PaymentIntentId}", command.PaymentIntentId);

        // Check if we already processed this payment
        var existingTransaction = await _context.PaymentTransaction
            .FirstOrDefaultAsync(t => t.StripePaymentIntentId == command.PaymentIntentId, cancellationToken);

        if (existingTransaction != null)
        {
            _logger.LogInformation("Payment transaction already exists for PaymentIntent: {PaymentIntentId}", command.PaymentIntentId);
            
            // Update status if it was previously incomplete
            if (existingTransaction.Status != PaymentStatus.Successful)
            {
                existingTransaction.Status = PaymentStatus.Successful;
                existingTransaction.PaidAt = DateTime.UtcNow;
                
                // Update user payment status
                var existingUser = await _context.User.FindAsync(existingTransaction.UserId, cancellationToken);
                if (existingUser != null)
                {
                    existingUser.UpdateSubscriptionStatus("active", _dateTime, command.CustomerId);
                }
                
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
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

        // Create new payment transaction
        var transaction = new PaymentTransaction
        {
            UserId = user.Id,
            StripePaymentIntentId = command.PaymentIntentId,
            StripeCustomerId = command.CustomerId,
            Amount = command.Amount,
            Currency = command.Currency,
            Status = PaymentStatus.Successful,
            PaidAt = DateTime.UtcNow
        };

        _context.PaymentTransaction.Add(transaction);

        // Update user payment status
        user.UpdateSubscriptionStatus("active", _dateTime, command.CustomerId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully processed payment for user {UserId}, PaymentIntent: {PaymentIntentId}", 
            user.Id, command.PaymentIntentId);
    }
}