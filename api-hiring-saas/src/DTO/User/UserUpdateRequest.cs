namespace DTO.User;

public sealed record UserUpdateRequest
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
}
