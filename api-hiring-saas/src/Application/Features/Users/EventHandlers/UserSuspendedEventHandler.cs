using Application.Common.MessageBroker;
using Domain.Events.Users;
using DTO.MessageBroker.Messages.Users;
using MediatR;

namespace Application.Features.Users.EventHandlers;

public sealed class UserSuspendedEventHandler : INotificationHandler<UserSuspendedEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public UserSuspendedEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;        
    }
    public async Task Handle(UserSuspendedEvent eventData, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new UserSuspendedMessage(eventData.User.Id));
    }
}