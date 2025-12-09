namespace DTO.Job.JobForm.JobFormSection;

public record JobFormSectionPropertyBase
{
    public string? Type { get; init; }
    public string? Label { get; init; }
    public bool Required { get; init; }
    public int Position { get; init; }
    public string? PlaceHolder { get; init; }
    public List<string>? Options { get; init; }
    public List<string>? InterviewRequired { get; init; }
}
