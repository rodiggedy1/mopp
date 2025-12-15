using Domain.Entities.JobApplications;
using Microsoft.AspNetCore.Http;

namespace Domain.Events.JobApplications;

public sealed class JobApplicationCreatedEvent : BaseEvent
{
    public JobApplicationCreatedEvent(JobApplication jobApplication, IFormFile? profilePicture, IFormFile? applicationVideo)
    {
        JobApplication = jobApplication;
        ProfilePicture = profilePicture;
        ApplicationVideo = applicationVideo;
    }

    public JobApplication JobApplication { get; } = null!;
    public IFormFile? ProfilePicture { get; }
    public IFormFile? ApplicationVideo { get; }
}