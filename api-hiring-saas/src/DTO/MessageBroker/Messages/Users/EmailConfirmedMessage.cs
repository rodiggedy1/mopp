namespace DTO.MessageBroker.Messages.Users;

public sealed record EmailConfirmedMessage(
    string FirstName,
    string LastName,
    string Email
    ) : MessageBase;
