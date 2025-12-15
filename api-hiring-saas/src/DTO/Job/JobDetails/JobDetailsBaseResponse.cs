using DTO.Job.JobForm;

namespace DTO.Job.JobDetails;

public record JobDetailsBaseResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string? Location { get; init; } = null!;
    public string EmploymentType { get; init; } = null!;
    public int Price { get; init; }
    public string UniqueHash { get; init; } = null!;
    public DateTime Created { get; init; }
    public JobFormResponse JobForm { get; init; } = new();
    public int CreatedBy { get; init; }
}
