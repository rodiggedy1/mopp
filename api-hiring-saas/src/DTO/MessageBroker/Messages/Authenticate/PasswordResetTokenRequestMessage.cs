namespace DTO.MessageBroker.Messages.Authenticate;

public sealed record PasswordResetTokenRequestMessage(string Email): MessageBase;