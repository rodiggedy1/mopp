using DTO.Enums.Notification;

namespace DTO.MessageBroker.Messages.Notification;

public sealed record CreateNotificationMessage(
    int UserId,
    string Title,
    string? Description,
    string Data,
    NotificationType Type) : MessageBase;
