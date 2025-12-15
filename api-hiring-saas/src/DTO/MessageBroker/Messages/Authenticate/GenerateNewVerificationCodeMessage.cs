namespace DTO.MessageBroker.Messages.Authenticate;

public sealed record GenerateNewVerificationCodeMessage(string Email) : MessageBase;
