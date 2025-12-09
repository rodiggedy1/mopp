namespace DTO.MessageBroker.Messages.Users;

public sealed record NoteFromCustomerMessage(
    string MessageType,
    string FirstName,
    string Email,
    string Note) : MessageBase;
