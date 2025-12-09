using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.Search;

namespace Application.Features.Users.Commands;

public sealed record UserInitiateSearchIndexRebuildCommand() : ICommand;

public sealed class UserInitiateSearchIndexRebuildCommandHandler : ICommandHandler<UserInitiateSearchIndexRebuildCommand>
{
    private readonly IMessagePublisher _messagePublisher;

    public UserInitiateSearchIndexRebuildCommandHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(UserInitiateSearchIndexRebuildCommand command, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new RebuilUserSearchIndexMessage());
    }
}
