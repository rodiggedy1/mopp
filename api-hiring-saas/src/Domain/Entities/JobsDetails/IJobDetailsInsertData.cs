namespace Domain.Entities.JobsDetails;

public interface IJobDetailsInsertData
{
    string Title { get; }
    string Description { get; }
    string? Location { get; }
    string EmploymentType { get; }
    int Price { get; }
    string UniqueHash { get; }

    int JobFormId { get; }
}
