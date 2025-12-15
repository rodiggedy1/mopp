namespace DTO.Payments;

public record SubscriptionStatusResult(
    string Status,
    DateTime? TrialEnd = null,
    DateTime? CurrentPeriodEnd = null,
    string? Error = null
);
