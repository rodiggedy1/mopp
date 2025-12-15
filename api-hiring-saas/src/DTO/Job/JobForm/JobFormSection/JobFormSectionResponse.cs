namespace DTO.Job.JobForm.JobFormSection;

public sealed record JobFormSectionResponse : JobFormSectionBase
{
    public JobFormSectionIconResponse? Icon { get; init; }
    public List<JobFormSectionPropertyResponse>? JobFormSectionProperties { get; init; }
}
