namespace DTO.Job.JobForm.JobFormSection;

public record JobFormSectionBase
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Code { get; init; }
    public int? Position { get; init; }
}
