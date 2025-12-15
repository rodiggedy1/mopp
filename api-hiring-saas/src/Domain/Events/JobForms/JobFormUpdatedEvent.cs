using Domain.Entities.JobForms;

namespace Domain.Events.JobForms;

public sealed class JobFormUpdatedEvent : BaseEvent
{
    public JobFormUpdatedEvent(JobForm jobForm)
    {
        JobForm = jobForm;
    }

    public JobForm JobForm { get; } = null!;
}