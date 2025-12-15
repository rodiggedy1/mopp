using Domain.Entities.Base;
using Domain.Entities.User;
using Domain.Events.Notifications;
using Domain.Interfaces;
using DTO.Enums.Notification;

namespace Domain.Entities.Notifications;

public class Notification : BaseEntity
{
    public int UserId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; } = null!;
    public string Data { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime Created { get; private set; }

    public virtual ApplicationUser User { get; set; } = null!;

    private Notification()
    {   
    }

    private Notification(
        int userId,
        IDateTime dateTimeProvider,
        NotificationType notificationType,
        string title,
        string description,
        string data)
    {
        UserId = userId;
        Title = title;
        Description = description;
        Type = notificationType;
        Status = NotificationStatus.Unread;
        Created = dateTimeProvider.Now;
        Data = data;
    }

    public static Notification Create(
        int userId, 
        IDateTime dateTimeProvider, 
        NotificationType notificationType,
        string title,
        string? description,
        string data)
    {
        Notification notification = new(userId, dateTimeProvider, notificationType, title, description!, data);

        notification.AddDomainEvent(new NotificationCreatedEvent(notification));

        return notification;
    }

    public void UpdateStatus(NotificationStatus status, bool raiseEvent = true)
    {
        Status = status;

        if(raiseEvent)
            AddDomainEvent(new NotificationStatusUpdatedEvent(this));
    }
}
