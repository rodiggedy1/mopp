using Domain.Entities.JobForms;

namespace Domain.Events.JobForms;

public sealed class JobFormCreatedEvent : BaseEvent
{
    public JobFormCreatedEvent(JobForm jobForm)
    {
        JobForm = jobForm;
    }

    public JobForm JobForm { get; } = null!;
}