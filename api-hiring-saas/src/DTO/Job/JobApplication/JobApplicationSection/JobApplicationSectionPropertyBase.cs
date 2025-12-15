using DTO.Job.JobForm.JobFormSection;

namespace DTO.Job.JobApplication.JobApplicationSection;

public record JobApplicationSectionPropertyBase : JobFormSectionPropertyBase
{
    public string? StringValue { get; init; }
    public int? IntegerValue { get; init; }
    public DateTime? DateTimeValue { get; init; }
    public bool? BooleanValue { get; init; }
}
