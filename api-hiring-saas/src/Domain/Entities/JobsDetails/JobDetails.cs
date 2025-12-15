using Domain.Entities.Base;
using Domain.Entities.JobApplications;
using Domain.Entities.JobForms;
using Domain.Events;

namespace Domain.Entities.JobsDetails;

public class JobDetails : BaseAuditableEntity, IHasDomainEvents
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? Location { get; private set; }
    public string EmploymentType { get; private set; } = null!;
    public int Price { get; private set; }
    public string UniqueHash { get; private set; }

    public int JobFormId { get; private set; }

    public JobForm JobForm { get; private set; } = null!;

    public virtual IReadOnlyCollection<JobApplication> JobApplications { get; private set; } = new List<JobApplication>();

    private JobDetails() { }
    private JobDetails(IJobDetailsInsertData data)
    {
        Title = data.Title;
        Description = data.Description;
        Location = data.Location;
        EmploymentType = data.EmploymentType;
        Price = data.Price;
        UniqueHash = data.UniqueHash;

        JobFormId = data.JobFormId;

        //AddDomainEvent(new JobDetailsCreatedEvent(this));
    }

    public static JobDetails Create(IJobDetailsInsertData data)
    {
        return new JobDetails(data);
    }
}
