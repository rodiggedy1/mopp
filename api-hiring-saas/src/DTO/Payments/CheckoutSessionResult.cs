namespace DTO.Payments;

public record CheckoutSessionResult(
    bool IsSuccess,
    string? Error = null,
    int? UserId = null,
    string? Email = null,
    string? SubscriptionStatus = null,
    string? SubscriptionId = null,
    string? CustomerId = null,
    DateTime? TrialEndsAt = null
);