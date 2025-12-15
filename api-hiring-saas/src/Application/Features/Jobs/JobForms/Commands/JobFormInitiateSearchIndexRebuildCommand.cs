using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.Search;

namespace Application.Features.Jobs.JobForms.Commands;

public sealed record JobFormInitiateSearchIndexRebuildCommand() : ICommand;

public sealed class JobFormInitiateSearchIndexRebuildCommandHandler : ICommandHandler<JobFormInitiateSearchIndexRebuildCommand>
{
    private readonly IMessagePublisher _messagePublisher;

    public JobFormInitiateSearchIndexRebuildCommandHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(JobFormInitiateSearchIndexRebuildCommand command, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new RebuildJobFormSearchIndexMessage());
    }
}