using Domain.Entities.Notifications;

namespace Domain.Events.Notifications;

public sealed class NotificationCreatedEvent : BaseEvent
{
    public NotificationCreatedEvent(Notification notification)
    {
        Notification = notification;
    }

    public Notification Notification { get; }
}
