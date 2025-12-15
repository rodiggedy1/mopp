using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.Search;

namespace Application.Features.Jobs.JobApplications.Commands;

public sealed record JobApplicationInitiateSearchIndexRebuildCommand() : ICommand;

public sealed class JobApplicationInitiateSearchIndexRebuildCommandHandler : ICommandHandler<JobApplicationInitiateSearchIndexRebuildCommand>
{
    private readonly IMessagePublisher _messagePublisher;
    public JobApplicationInitiateSearchIndexRebuildCommandHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }
    public async Task Handle(JobApplicationInitiateSearchIndexRebuildCommand command, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new RebuildJobApplicationSearchIndexMessage());
    }
}