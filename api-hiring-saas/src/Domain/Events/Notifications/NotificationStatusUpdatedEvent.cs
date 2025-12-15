using Domain.Entities.Notifications;

namespace Domain.Events.Notifications;

public sealed class NotificationStatusUpdatedEvent : BaseEvent
{
    public NotificationStatusUpdatedEvent(Notification notification)
    {
        Notification = notification;
    }
    public Notification Notification { get; }
}
