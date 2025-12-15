namespace DTO.Payments;

public record UserPaymentStatusResponse
{
    public bool HasActivePaidSubscription { get; init; }
    public DateTime? LastPaymentDate { get; init; }
    public DateTime? NextPaymentDueDate { get; init; }
    public string? StripeCustomerId { get; init; }
    public bool IsPaymentOverdue { get; init; }
    
    // Trial information
    public bool HasUsedTrial { get; init; }
    public bool IsCurrentlyInTrial { get; init; }
    public DateTime? TrialStartedAt { get; init; }
    public DateTime? TrialEndsAt { get; init; }
    public string? SubscriptionStatus { get; init; }
    
    public List<PaymentTransactionResponse> RecentTransactions { get; init; } = new();
}