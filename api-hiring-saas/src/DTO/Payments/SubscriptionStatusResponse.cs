namespace DTO.Payments;

public record SubscriptionStatusResponse
{
    public string Status { get; init; } = null!;
    public DateTime? TrialEnd { get; init; }
    public DateTime? CurrentPeriodEnd { get; init; }
    public string? Error { get; init; }
}