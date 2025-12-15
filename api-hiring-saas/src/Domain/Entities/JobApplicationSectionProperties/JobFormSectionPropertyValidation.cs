using System.Text.Json.Serialization;

namespace Domain.Entities.JobApplicationSectionProperties;

public class JobApplicationSectionPropertyValidation
{
    [JsonConstructor]
    public JobApplicationSectionPropertyValidation()
    {
    }

    private JobApplicationSectionPropertyValidation(string? pattern)
    {
        Pattern = pattern;
    }

    public string? Pattern { get; set; }

    public static JobApplicationSectionPropertyValidation Create(string? pattern) =>
         new(pattern);

    public void Update(string? pattern)
    {
        Pattern = pattern;
    }
}
