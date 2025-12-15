using Application.Common.MessageBroker;
using Application.Features.Jobs.JobApplications.Commands;
using Domain.Events.JobApplications;
using DTO.Enums.Job.JobApplication;
using MediatR;

namespace Application.Features.Jobs.JobApplications.EventHandlers;

public sealed class JobApplicationCreatedEventHandler : INotificationHandler<JobApplicationCreatedEvent>
{
    private readonly ISender _mediatr;

    public JobApplicationCreatedEventHandler(
        ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Handle(JobApplicationCreatedEvent eventData, CancellationToken cancellationToken)
    {
        if (eventData.ProfilePicture != null)
        {
            await _mediatr.Send(new JobApplicationFileUploadCommand(
                eventData.JobApplication.Id,
                eventData.ProfilePicture,
                JobApplicationFileType.ProfilePicture));
        }

        if (eventData.ApplicationVideo != null)
        {
            await _mediatr.Send(new JobApplicationFileUploadCommand(
                eventData.JobApplication.Id,
                eventData.ApplicationVideo,
                JobApplicationFileType.ApplicationVideo));
        }

        await _mediatr.Send(new JobApplicationIndexCommand(eventData.JobApplication.Id), cancellationToken);
    }
}
