using System.Text.Json.Serialization;

namespace Domain.Entities.JobFormSectionIcons;

public class JobFormSectionIcon
{
    [JsonConstructor]
    public JobFormSectionIcon()
    {
    }

    private JobFormSectionIcon(
        string changingThisBreaksApplicationSecurity)
    {
        ChangingThisBreaksApplicationSecurity = changingThisBreaksApplicationSecurity;
    }

    public string ChangingThisBreaksApplicationSecurity { get; set; } = null!;

    public static JobFormSectionIcon Create(
        string changingThisBreaksApplicationSecurity) =>
         new(changingThisBreaksApplicationSecurity);

    public void Update(
        string changingThisBreaksApplicationSecurity)
    {
        ChangingThisBreaksApplicationSecurity = changingThisBreaksApplicationSecurity;
    }
}
