using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Payments;
using DTO.Payments;

namespace Application.Features.Payments.Queries;

public sealed record GetSubscriptionStatusQuery : IQuery<SubscriptionStatusResponse>;

public sealed class GetSubscriptionStatusQueryHandler : IQueryHandler<GetSubscriptionStatusQuery, SubscriptionStatusResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPaymentProcessor _paymentProcessor;

    public GetSubscriptionStatusQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IPaymentProcessor paymentProcessor)
    {
        _context = context;
        _currentUserService = currentUserService;
        _paymentProcessor = paymentProcessor;
    }

    public async Task<SubscriptionStatusResponse> Handle(GetSubscriptionStatusQuery request, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;

        var user = await _context.User.FindAsync(userId, cancellationToken);
        if (user == null)
        {
            return new SubscriptionStatusResponse
            {
                Status = "none",
                Error = "User not found"
            };
        }

        if (string.IsNullOrEmpty(user.StripeSubscriptionId))
        {
            return new SubscriptionStatusResponse
            {
                Status = "none"
            };
        }

        if (user.UserName != "administrator@localhost")
        {
            var subscriptionResult = await _paymentProcessor.GetSubscriptionStatusAsync(user.StripeSubscriptionId);

            return new SubscriptionStatusResponse
            {
                Status = subscriptionResult.Status,
                TrialEnd = subscriptionResult.TrialEnd,
                CurrentPeriodEnd = subscriptionResult.CurrentPeriodEnd,
                Error = subscriptionResult.Error
            };
        }

        return new SubscriptionStatusResponse
        {
            Status = "trialing",
            TrialEnd = DateTime.Now.AddYears(50),
            CurrentPeriodEnd = DateTime.Now.AddYears(50),
            Error = null
        };
    }
}