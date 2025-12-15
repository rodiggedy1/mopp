namespace Infrastructure.Identity.Configuration;

public class WorkerAuthConfig
{
    public static string SectionName = "WorkerAuth";
    public string Secret { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int TokenValidityInMinutes { get; set; }
    public TimeSpan TokenValidity => TimeSpan.FromMinutes(TokenValidityInMinutes);
    public string ValidIssuer { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
}
