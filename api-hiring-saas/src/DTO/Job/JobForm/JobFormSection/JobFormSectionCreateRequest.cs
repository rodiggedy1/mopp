namespace DTO.Job.JobForm.JobFormSection;

public sealed record JobFormSectionCreateRequest : JobFormSectionBase
{
    public JobFormSectionIconCreateRequest? Icon { get; init; }
    public List<JobFormSectionPropertyCreateRequest>? JobFormSectionProperties { get; init; }
}
