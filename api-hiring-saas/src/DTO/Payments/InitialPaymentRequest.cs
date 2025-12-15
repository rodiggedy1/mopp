namespace DTO.Payments;

public record InitialPaymentRequest
{
    public string StripePaymentIntentId { get; init; } = null!;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "usd";
}