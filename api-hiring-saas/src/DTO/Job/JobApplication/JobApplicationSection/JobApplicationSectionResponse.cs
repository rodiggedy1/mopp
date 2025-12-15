using DTO.Job.JobForm.JobFormSection;

namespace DTO.Job.JobApplication.JobApplicationSection;

public sealed record JobApplicationSectionResponse : JobApplicationSectionBase
{
    public JobApplicationSectionIconResponse? Icon { get; init; }
    public List<JobApplicationSectionPropertyResponse>? JobApplicationSectionProperties { get; init; }
}
