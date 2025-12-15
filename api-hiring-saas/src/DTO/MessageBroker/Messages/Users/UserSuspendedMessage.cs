namespace DTO.MessageBroker.Messages.Users;

public sealed record UserSuspendedMessage(int UserId) : MessageBase;
