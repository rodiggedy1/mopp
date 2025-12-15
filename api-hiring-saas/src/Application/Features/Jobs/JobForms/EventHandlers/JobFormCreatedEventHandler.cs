using Application.Features.Jobs.JobForms.Commands;
using Domain.Events.JobForms;
using MediatR;

namespace Application.Features.Jobs.JobForms.EventHandlers;

public sealed class JobFormCreatedEventHandler : INotificationHandler<JobFormCreatedEvent>
{
    private readonly ISender _mediatr;

    public JobFormCreatedEventHandler(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public async Task Handle(JobFormCreatedEvent eventData, CancellationToken cancellationToken)
    {
        await _mediatr.Send(new JobFormIndexCommand(eventData.JobForm.Id), cancellationToken);
    }
}