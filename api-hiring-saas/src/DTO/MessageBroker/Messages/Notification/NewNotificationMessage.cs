using DTO.Enums.Notification;

namespace DTO.MessageBroker.Messages.Notification;

public sealed record NewNotificationMessage(
    int NotificationId,
    int UserId,
    string Title,
    string? Description,
    NotificationType Type,
    NotificationStatus Status,
    DateTime DateSent,
    string Data) : MessageBase;