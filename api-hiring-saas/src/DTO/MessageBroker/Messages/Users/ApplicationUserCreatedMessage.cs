namespace DTO.MessageBroker.Messages.Users;

public sealed record ApplicationUserCreatedMessage(int UserId) : MessageBase;
