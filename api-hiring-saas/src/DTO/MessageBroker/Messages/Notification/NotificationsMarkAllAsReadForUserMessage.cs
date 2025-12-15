namespace DTO.MessageBroker.Messages.Notification;

public sealed record NotificationsMarkAllAsReadForUserMessage(IReadOnlyCollection<int> NotificationIds) : MessageBase;
