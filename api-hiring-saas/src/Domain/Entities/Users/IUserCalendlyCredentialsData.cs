namespace Domain.Entities.Users;

public interface IUserCalendlyCredentialsData
{    
    string? CalendlyClientId { get; }
    string? CalendlyClientSecret { get; }
    string? CalendlyAccessToken { get; }
    string? CalendlyCode { get; }
    string? CalendlyRedirectUri { get; }
    string? CalendlyRefreshToken { get; }
    string? CalendlyTokenExpiresAt { get; }
}
