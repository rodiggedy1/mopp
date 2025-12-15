namespace DTO.MessageBroker.Messages.Users;

public sealed record PasswordChangedMessage(
string FirstName,
string LastName,
string Email
) : MessageBase;
