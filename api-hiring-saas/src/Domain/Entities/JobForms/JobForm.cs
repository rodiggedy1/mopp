using Domain.Entities.Base;
using Domain.Entities.JobFormSections;
using Domain.Entities.JobsDetails;
using Domain.Events;
using Domain.Events.JobForms;

namespace Domain.Entities.JobForms;

public class JobForm : BaseAuditableEntity, IHasDomainEvents
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string UniqueHash { get; private set; } = null!;

    public virtual IReadOnlyCollection<JobDetails> JobsDetails { get; private set; } = new List<JobDetails>();
    public virtual IReadOnlyCollection<JobFormSection> JobFormSections { get; private set; } = new List<JobFormSection>();

    private JobForm() { }
    private JobForm(IJobFormInsertData data)
    {
        Title = data.Title;
        Description = data.Description;
        UniqueHash = data.UniqueHash;

        AddDomainEvent(new JobFormCreatedEvent(this));
    }

    public static JobForm Create(IJobFormInsertData data)
    {
        return new JobForm(data);
    }

    public void Update(IJobFormUpdateData data)
    {
        Title = data.Title;
        Description = data.Description;

        AddDomainEvent(new JobFormUpdatedEvent(this));
    }

    public void SetSections(List<JobFormSection> sections)
    {
        JobFormSections = sections;
    }
}
