using DTO.Enums.Payments;

namespace DTO.Payments;

public record PaymentTransactionResponse
{
    public int Id { get; init; }
    public string StripePaymentIntentId { get; init; } = null!;
    public string? StripeCustomerId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
    public PaymentStatus Status { get; init; }
    public string? FailureReason { get; init; }
    public DateTime? PaidAt { get; init; }
    public string? StripeInvoiceId { get; init; }
    public DateTime Created { get; init; }
}