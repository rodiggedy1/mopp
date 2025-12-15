using System.Text.Json.Serialization;

namespace Domain.Entities.JobFormSectionProperties;

public class JobFormSectionProperty
{
    [JsonConstructor]
    public JobFormSectionProperty()
    {
    }

    private JobFormSectionProperty(
        string? type, 
        string? label, 
        bool required, 
        int position, 
        string? placeHolder, 
        List<string>? options,
        List<string>? interviewRequired,
        JobFormSectionPropertyValidation validation)
    {
        Type = type;
        Label = label;
        Required = required;
        Position = position;
        PlaceHolder = placeHolder;
        Options = options;
        InterviewRequired = interviewRequired;
        Validation = validation;
    }

    public string? Type { get; set; }
    public string? Label { get; set; }
    public bool Required { get; set; }
    public int Position { get; set; }
    public string? PlaceHolder { get; set; }
    public List<string>? Options { get; set; }
    public List<string>? InterviewRequired { get; set; }
    public JobFormSectionPropertyValidation? Validation { get; set; }

    public static JobFormSectionProperty Create(
        string? type,
        string? label,
        bool required,
        int position,
        string? placeHolder,
        List<string>? options,
        List<string>? interviewRequired,
        JobFormSectionPropertyValidation validation) =>
         new(type, label, required, position, placeHolder, options, interviewRequired, validation);

    public void Update(
        string? type,
        string? label,
        bool required,
        int position,
        string? placeHolder,
        List<string>? options,
        List<string>? interviewRequired,
        JobFormSectionPropertyValidation validation)
    {
        Type = type;
        Label = label;
        Required = required;
        Position = position;
        PlaceHolder = placeHolder;
        Options = options;
        InterviewRequired = interviewRequired;
        Validation = validation;
    }
}
