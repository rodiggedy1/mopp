using DTO.Job.JobForm.JobFormSection;

namespace DTO.Job.JobApplication.JobApplicationSection;

public record JobApplicationSectionPropertyCreateRequest : JobApplicationSectionPropertyBase
{
    public JobApplicationSectionPropertyValidationCreateRequest? Validation { get; init; }
}
