using DTO.Job.JobForm.JobFormSection;

namespace DTO.Job.JobApplication.JobApplicationSection;

public sealed record JobApplicationSectionPropertyResponse : JobApplicationSectionPropertyBase
{
    public JobApplicationSectionPropertyValidationResponse? Validation { get; init; }
}
