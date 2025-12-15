namespace DTO.MessageBroker.Messages.Authenticate;

public sealed record ResendVerificationEmailMessage: MessageBase
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string EmailVerificationCode { get; init; } = null!;
    public Guid Uid { get; init; }
}
