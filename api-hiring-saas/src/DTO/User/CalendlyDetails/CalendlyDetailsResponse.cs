namespace DTO.User.CalendlyDetails;

public sealed record CalendlyDetailsResponse
{
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public string? AccessToken { get; init; }
    public string? Code { get; init; }
    public string? RedirectUri { get; init; }
    public string? RefreshToken { get; init; }
    public string? TokenExpiresAt { get; init; }
}
