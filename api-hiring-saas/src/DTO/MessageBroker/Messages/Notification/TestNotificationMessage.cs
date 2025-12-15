namespace DTO.MessageBroker.Messages.Notification;

public sealed record TestNotificationMessage(
    int UserId,
    string Text) : MessageBase;
