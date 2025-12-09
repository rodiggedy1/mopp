using Application.Features.Jobs.JobApplications.Commands;
using Domain.Events.JobApplications;
using MediatR;

namespace Application.Features.Jobs.JobApplications.EventHandlers;

public sealed class JobApplicationUpdatedEventHandler : INotificationHandler<JobApplicationUpdatedEvent>
{
    private readonly ISender _mediatr;

    public JobApplicationUpdatedEventHandler(
        ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Handle(JobApplicationUpdatedEvent eventData, CancellationToken cancellationToken)
    {
        await _mediatr.Send(new JobApplicationIndexCommand(eventData.JobApplication.Id), cancellationToken);
    }
}