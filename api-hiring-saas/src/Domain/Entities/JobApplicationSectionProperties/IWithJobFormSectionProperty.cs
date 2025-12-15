namespace Domain.Entities.JobApplicationSectionProperties;

public interface IWithJobApplicationSectionProperty
{
    List<JobApplicationSectionProperty>? JobApplicationSectionProperties { get; set; }
}
