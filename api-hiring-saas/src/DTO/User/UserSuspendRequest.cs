namespace DTO.User;

public sealed record UserSuspendRequest
{
    public string SuspensionReason { get; init; } = null!;
}
