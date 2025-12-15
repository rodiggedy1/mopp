namespace DTO.Payments;

public record CreateCheckoutSessionRequest
{
    public string SuccessUrl { get; init; } = null!;
    public string CancelUrl { get; init; } = null!;
}