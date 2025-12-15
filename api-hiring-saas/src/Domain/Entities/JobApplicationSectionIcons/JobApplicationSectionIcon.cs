using System.Text.Json.Serialization;

namespace Domain.Entities.JobApplicationSectionIcons;

public class JobApplicationSectionIcon
{
    [JsonConstructor]
    public JobApplicationSectionIcon()
    {
    }

    private JobApplicationSectionIcon(
        string changingThisBreaksApplicationSecurity)
    {
        ChangingThisBreaksApplicationSecurity = changingThisBreaksApplicationSecurity;
    }

    public string ChangingThisBreaksApplicationSecurity { get; set; } = null!;

    public static JobApplicationSectionIcon Create(
        string changingThisBreaksApplicationSecurity) =>
         new(changingThisBreaksApplicationSecurity);

    public void Update(
        string changingThisBreaksApplicationSecurity)
    {
        ChangingThisBreaksApplicationSecurity = changingThisBreaksApplicationSecurity;
    }
}
