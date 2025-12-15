using Application.Common.MessageBroker;
using Domain.Events.Notifications;
using DTO.MessageBroker.Messages.Notification;
using DTO.MessageBroker.Messages.Search;
using MediatR;

namespace Application.Features.Notifications.EventHandlers;

public sealed class NotificationCreatedEventHandler : INotificationHandler<NotificationCreatedEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public NotificationCreatedEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }
    public async Task Handle(NotificationCreatedEvent eventData, CancellationToken cancellationToken)
    {
        await _messagePublisher.PublishAsync(new IndexNotificationMessage(eventData.Notification.Id));

        await _messagePublisher.PublishAsync(new NewNotificationMessage(
           eventData.Notification.Id,
           eventData.Notification.UserId,
           eventData.Notification.Title,
           eventData.Notification.Description,
           eventData.Notification.Type,
           eventData.Notification.Status,
           eventData.Notification.Created,
           eventData.Notification.Data));
    }
}
