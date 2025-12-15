using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Payments;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Queries;

public sealed record GetUserPaymentStatusQuery : IQuery<UserPaymentStatusResponse>;

public sealed class GetUserPaymentStatusQueryHandler : IQueryHandler<GetUserPaymentStatusQuery, UserPaymentStatusResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetUserPaymentStatusQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<UserPaymentStatusResponse> Handle(GetUserPaymentStatusQuery request, CancellationToken cancellationToken)
    {
        var userId = (int)_currentUserService.UserId!;
        
        var user = await _context.User.FindAsync(userId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var transactions = await _context.PaymentTransaction
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Created)
            .Take(10)
            .ToListAsync(cancellationToken);

        // Determine trial usage
        bool hasUsedTrial = user.TrialStartedAt.HasValue;
        bool isCurrentlyInTrial = user.IsTrialActive();
        bool isTrialExpired = hasUsedTrial && user.TrialEndsAt.HasValue && DateTime.UtcNow > user.TrialEndsAt.Value;

        return new UserPaymentStatusResponse
        {
            HasActivePaidSubscription = user.IsSubscriptionActive(),
            LastPaymentDate = user.SubscriptionStartedAt,
            NextPaymentDueDate = user.TrialEndsAt ?? user.SubscriptionEndsAt,
            StripeCustomerId = user.StripeCustomerId,
            IsPaymentOverdue = !user.IsSubscriptionActive(),
            
            // Trial information
            HasUsedTrial = hasUsedTrial,
            IsCurrentlyInTrial = isCurrentlyInTrial,
            TrialStartedAt = user.TrialStartedAt,
            TrialEndsAt = user.TrialEndsAt,
            SubscriptionStatus = user.SubscriptionStatus,
            
            RecentTransactions = _mapper.Map<List<PaymentTransactionResponse>>(transactions)
        };
    }
}