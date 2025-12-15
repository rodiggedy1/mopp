namespace DTO.Payments;

public record VerifyPaymentResponse
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public int? UserId { get; init; }
    public string? Email { get; init; }
    public string? SubscriptionStatus { get; init; }
    public string? SubscriptionId { get; init; }
    public DateTime? TrialEndsAt { get; init; }
}