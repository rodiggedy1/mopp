using Domain.Entities.Base;
using Domain.Entities.JobForms;
using Domain.Entities.JobFormSectionIcons;
using Domain.Entities.JobFormSectionProperties;

namespace Domain.Entities.JobFormSections;

public class JobFormSection : BaseEntity, IWithJobFormSectionProperty, IWithJobFormSectionIcon
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public string? Code { get; private set; }
    public int? Position { get; private set; }
    public int JobFormId { get; private set; }
    public JobFormSectionIcon? Icon { get; set; }
    public List<JobFormSectionProperty>? JobFormSectionProperties { get; set; }

    public virtual JobForm JobForm { get; private set; } = null!;

    public JobFormSection()
    {

    }
    private JobFormSection(IJobFormSectionInsertData data)
    {
        Name = data.Name;
        Description = data.Description;
        Code = data.Code;
        Position = data.Position;
        JobFormSectionProperties = data.JobFormSectionProperties;
        Icon = data.Icon;
    }

    public static JobFormSection Create(IJobFormSectionInsertData data)
    {
        return new JobFormSection(data);
    }

}
