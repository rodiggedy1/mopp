using Domain.Entities.JobApplicationSectionIcons;
using Domain.Entities.JobApplicationSectionProperties;

namespace Domain.Entities.JobApplicationSections;

public interface IJobApplicationSectionInsertData
{
    public string? Name { get; }
    public string? Description { get; }
    public string? Code { get; }
    public int? Position { get; }
    public JobApplicationSectionIcon? Icon { get; }
    public List<JobApplicationSectionProperty>? JobApplicationSectionProperties { get; }
}
