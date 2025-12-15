using Domain.Entities.JobFormSectionProperties;

namespace Domain.Entities.JobFormSections;

public interface IWithJobFormSectionProperty
{
    List<JobFormSectionProperty>? JobFormSectionProperties { get; set; }
}
