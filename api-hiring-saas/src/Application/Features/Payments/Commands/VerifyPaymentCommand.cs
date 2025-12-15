using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Payments;
using Domain.Interfaces;
using DTO.Payments;
using FluentValidation;

namespace Application.Features.Payments.Commands;

public sealed record VerifyPaymentCommand(
    string SessionId
) : ICommand<VerifyPaymentResponse>;

public sealed class VerifyPaymentCommandHandler : ICommandHandler<VerifyPaymentCommand, VerifyPaymentResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IDateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyPaymentCommandHandler(
        IApplicationDbContext context,
        IPaymentProcessor paymentProcessor,
        IDateTime dateTime,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _paymentProcessor = paymentProcessor;
        _dateTime = dateTime;
        _unitOfWork = unitOfWork;
    }

    public async Task<VerifyPaymentResponse> Handle(VerifyPaymentCommand command, CancellationToken cancellationToken)
    {
        var result = await _paymentProcessor.VerifyCheckoutSessionAsync(command.SessionId);

        if (!result.IsSuccess)
        {
            return new VerifyPaymentResponse
            {
                Success = false,
                Error = result.Error
            };
        }

        // Update user with subscription information
        var user = await _context.User.FindAsync(result.UserId!.Value, cancellationToken);
        if (user == null)
        {
            return new VerifyPaymentResponse
            {
                Success = false,
                Error = "User not found"
            };
        }

        // Update user subscription details
        user.UpdateSubscriptionStatus(result.SubscriptionStatus!, _dateTime, result.CustomerId, result.SubscriptionId);

        if (result.TrialEndsAt.HasValue)
        {
            if (result.CustomerId == null || result.SubscriptionId == null)
            {
                return new VerifyPaymentResponse
                {
                    Success = false,
                    Error = "Missing CustomerId or SubscriptionId for trial start"
                };
            }

            user.StartTrial(DateTime.UtcNow, result.TrialEndsAt.Value, result.CustomerId!, result.SubscriptionId!);
        }
        
        if (result.SubscriptionStatus == "active")
        {
            user.StartSubscription(DateTime.UtcNow);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new VerifyPaymentResponse
        {
            Success = true,
            UserId = result.UserId,
            Email = result.Email,
            SubscriptionStatus = result.SubscriptionStatus,
            SubscriptionId = result.SubscriptionId,
            TrialEndsAt = result.TrialEndsAt
        };
    }
}

public sealed class VerifyPaymentCommandValidator : AbstractValidator<VerifyPaymentCommand>
{
    public VerifyPaymentCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty()
            .WithMessage("Session ID is required");
    }
}