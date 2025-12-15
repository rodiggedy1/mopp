namespace DTO.Job.JobForm.JobFormSection;

public sealed record JobFormSectionPropertyResponse : JobFormSectionPropertyBase
{
    public JobFormSectionPropertyValidationResponse? Validation { get; init; }
}
