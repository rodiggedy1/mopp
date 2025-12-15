using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.Search;

namespace Application.Features.Notifications.Commands
{
    public sealed record NotificationInitiateSearchIndexRebuildCommand() : ICommand;

    public sealed class NotificationInitiateSearchIndexRebuildCommandHandler : ICommandHandler<NotificationInitiateSearchIndexRebuildCommand>
    {
        private readonly IMessagePublisher _messagePublisher;

        public NotificationInitiateSearchIndexRebuildCommandHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(NotificationInitiateSearchIndexRebuildCommand command, CancellationToken cancellationToken)
        {
            await _messagePublisher.PublishAsync(new RebuilNotificationIndexMessage());
        }
    }

}
