namespace Domain.Entities.JobForms;

public interface IJobFormInsertData : IJobFormBaseData
{
    string? UniqueHash { get; }
}
