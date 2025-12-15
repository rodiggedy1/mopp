namespace DTO.MessageBroker.Messages.Users;

public sealed record UserVerificationStatusUpdatedMessage(int UserId): MessageBase;
