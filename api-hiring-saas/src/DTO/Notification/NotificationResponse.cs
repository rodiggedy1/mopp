using DTO.Response;

namespace DTO.Notification;

public record NotificationResponse
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; } = null!;
    public string Data { get; init; } = null!;
    public ListItemBaseResponse Type { get; init; } = null!;
    public ListItemBaseResponse Status { get; init; } = null!;
    public DateTime Created { get; init; }
}
