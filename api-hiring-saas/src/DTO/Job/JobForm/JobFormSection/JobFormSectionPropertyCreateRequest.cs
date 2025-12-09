namespace DTO.Job.JobForm.JobFormSection;

public sealed record JobFormSectionPropertyCreateRequest : JobFormSectionPropertyBase
{
    public JobFormSectionPropertyValidationCreateRequest? Validation { get; init; }
}
