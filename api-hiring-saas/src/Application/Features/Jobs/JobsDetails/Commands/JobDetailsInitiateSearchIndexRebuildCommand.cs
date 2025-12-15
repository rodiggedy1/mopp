using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.Search;

namespace Application.Features.Jobs.JobsDetails.Commands;

public sealed record JobDetailsInitiateSearchIndexRebuildCommand() : ICommand;

public sealed class JobDetailsInitiateSearchIndexRebuildCommandHandler : ICommandHandler<JobDetailsInitiateSearchIndexRebuildCommand>
{
    private readonly IMessagePublisher _messagePublisher;
    public JobDetailsInitiateSearchIndexRebuildCommandHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }
    public async Task Handle(JobDetailsInitiateSearchIndexRebuildCommand command, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new RebuildJobDetailsSearchIndexMessage());
    }
}