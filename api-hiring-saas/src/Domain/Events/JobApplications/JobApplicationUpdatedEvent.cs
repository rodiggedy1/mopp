using Domain.Entities.JobApplications;

namespace Domain.Events.JobApplications;

public sealed class JobApplicationUpdatedEvent : BaseEvent
{
    public JobApplicationUpdatedEvent(JobApplication jobApplication)
    {
        JobApplication = jobApplication;
    }

    public JobApplication JobApplication { get; } = null!;
}