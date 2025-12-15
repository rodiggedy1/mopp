using System.Text.Json.Serialization;

namespace Domain.Entities.JobApplicationSectionProperties;

public class JobApplicationSectionProperty
{
    [JsonConstructor]
    public JobApplicationSectionProperty()
    {
    }

    private JobApplicationSectionProperty(
        string? type, 
        string? label, 
        bool required, 
        int position, 
        string? placeHolder, 
        List<string>? options, 
        List<string>? interviewRequired,
        string? stringValue,
        int? integerValue,
        DateTime? dateTimeValue,
        bool? booleanValue,
        JobApplicationSectionPropertyValidation validation)
    {
        Type = type;
        Label = label;
        Required = required;
        Position = position;
        PlaceHolder = placeHolder;
        Options = options;
        InterviewRequired = interviewRequired;
        Validation = validation;
        StringValue = stringValue;
        IntegerValue = integerValue;
        DateTimeValue = dateTimeValue;
        BooleanValue = booleanValue;
    }

    public string? Type { get; set; }
    public string? Label { get; set; }
    public bool Required { get; set; }
    public int Position { get; set; }
    public string? PlaceHolder { get; set; }
    public List<string>? Options { get; set; }
    public List<string>? InterviewRequired { get; set; }
    public string? StringValue { get; set; }
    public int? IntegerValue { get; set; }
    public DateTime? DateTimeValue { get; set; }
    public bool? BooleanValue { get; set; }
    public JobApplicationSectionPropertyValidation? Validation { get; set; }

    public static JobApplicationSectionProperty Create(
        string? type,
        string? label,
        bool required,
        int position,
        string? placeHolder,
        List<string>? options,
        List<string>? interviewRequired,
        string? stringValue,
        int? integerValue,
        DateTime? dateTimeValue,
        bool booleanValue,
        JobApplicationSectionPropertyValidation validation) =>
         new(type, label, required, position, placeHolder, options, interviewRequired, stringValue, integerValue, dateTimeValue, booleanValue, validation);

    public void Update(
        string? type,
        string? label,
        bool required,
        int position,
        string? placeHolder,
        List<string>? options,
        List<string>? interviewRequired,
        string? stringValue,
        int? integerValue,
        DateTime? dateTimeValue,
        bool? booleanValue,
        JobApplicationSectionPropertyValidation validation)
    {
        Type = type;
        Label = label;
        Required = required;
        Position = position;
        PlaceHolder = placeHolder;
        Options = options;
        InterviewRequired = interviewRequired;
        StringValue = stringValue;
        IntegerValue = integerValue;
        DateTimeValue = dateTimeValue;
        BooleanValue = booleanValue;
        Validation = validation;
    }
}
