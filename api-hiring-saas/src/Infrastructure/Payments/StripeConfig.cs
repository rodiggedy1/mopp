namespace Infrastructure.Payments;

public sealed record StripeConfig
{
    public const string SectionName = "StripeConfig";
    public string Key { get; init; } = null!;
    public string WebhookSecret { get; init; } = null!;
    public string SubscriptionPriceId { get; init; } = null!;
    public string SuccessUrl { get; init; } = null!;
    public string CancelUrl { get; init; } = null!;
    public string SubscriptionUrl { get; init; } = null!;
}