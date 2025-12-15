using Domain.Entities.JobFormSectionIcons;
using Domain.Entities.JobFormSectionProperties;

namespace Domain.Entities.JobFormSections;

public interface IJobFormSectionInsertData
{
    public string? Name { get; }
    public string? Description { get; }
    public string? Code { get; }
    public int? Position { get; }
    public JobFormSectionIcon? Icon { get; }
    public List<JobFormSectionProperty>? JobFormSectionProperties { get; }
}
