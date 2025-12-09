using DTO.Response;

namespace DTO.User;

public record UserResponse : UserBaseResponse
{
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public string? SuspensionReason { get; init; }
    public DateTime DateCreated { get; init; }
    public ListItemBaseResponse Status { get; init; } = null!;
}

