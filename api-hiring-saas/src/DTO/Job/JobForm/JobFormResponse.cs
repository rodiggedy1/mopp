using DTO.Job.JobForm.JobFormSection;

namespace DTO.Job.JobForm;

public record JobFormResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string UniqueHash { get; init; } = null!;
    public string Description { get; init; } = null!;
    public int CreatedBy { get; init; }
    public IReadOnlyCollection<JobFormSectionResponse>? JobFormSections { get; set; }
}
