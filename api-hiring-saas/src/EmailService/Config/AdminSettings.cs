namespace EmailService.Config;

public class AdminSettings
{
    public const string SectionName = "AdminSettings";
    public string AdminEmail { get; set; } = null!;
    public List<string>? SystemEmails { get; set; }
}