using Application.Features.Jobs.JobForms.Commands;
using Domain.Events.JobForms;
using MediatR;

namespace Application.Features.Jobs.JobForms.EventHandlers;

public sealed class JobFormUpdatedEventHandler : INotificationHandler<JobFormUpdatedEvent>
{
    private readonly ISender _mediatr;

    public JobFormUpdatedEventHandler(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Handle(JobFormUpdatedEvent eventData, CancellationToken cancellationToken)
    {
        await _mediatr.Send(new JobFormIndexCommand(eventData.JobForm.Id), cancellationToken);
    }
}