using Domain.Entities.Base;
using Domain.Entities.JobApplications;
using Domain.Entities.JobApplicationSectionIcons;
using Domain.Entities.JobApplicationSectionProperties;

namespace Domain.Entities.JobApplicationSections;

public class JobApplicationSection : BaseEntity, IWithJobApplicationSectionProperty, IWithJobApplicationSectionIcon
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public string? Code { get; private set; }
    public int? Position { get; private set; }
    public int JobApplicationId { get; private set; }
    public JobApplicationSectionIcon? Icon { get; set; }
    public List<JobApplicationSectionProperty>? JobApplicationSectionProperties { get; set; }

    public virtual JobApplication JobApplication { get; private set; } = null!;

    public JobApplicationSection()
    {

    }
    private JobApplicationSection(IJobApplicationSectionInsertData data)
    {
        Name = data.Name;
        Description = data.Description;
        Code = data.Code;
        Position = data.Position;
        JobApplicationSectionProperties = data.JobApplicationSectionProperties;
        Icon = data.Icon;
    }

    public static JobApplicationSection Create(IJobApplicationSectionInsertData data)
    {
        return new JobApplicationSection(data);
    }

}
