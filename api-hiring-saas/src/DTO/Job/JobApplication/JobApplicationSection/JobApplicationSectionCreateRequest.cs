namespace DTO.Job.JobApplication.JobApplicationSection;

public sealed record JobApplicationSectionCreateRequest : JobApplicationSectionBase
{
    public JobApplicationSectionIconCreateRequest? Icon { get; init; }
    public List<JobApplicationSectionPropertyCreateRequest>? JobApplicationSectionProperties { get; init; }

}
