using System.Text.Json.Serialization;

namespace Domain.Entities.JobFormSectionProperties;

public class JobFormSectionPropertyValidation
{
    [JsonConstructor]
    public JobFormSectionPropertyValidation()
    {
    }

    private JobFormSectionPropertyValidation(string? pattern)
    {
        Pattern = pattern;
    }

    public string? Pattern { get; set; }

    public static JobFormSectionPropertyValidation Create(string? pattern) =>
         new(pattern);

    public void Update(string? pattern)
    {
        Pattern = pattern;
    }
}
