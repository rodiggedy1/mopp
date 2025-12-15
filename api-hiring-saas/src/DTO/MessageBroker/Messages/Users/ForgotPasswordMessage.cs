using DTO.Enums;

namespace DTO.MessageBroker.Messages.Users;

public sealed record ForgotPasswordMessage(
    string FirstName,
    string LastName,
    string Email,
    string Token,
    Guid Uid
    ) : MessageBase;
