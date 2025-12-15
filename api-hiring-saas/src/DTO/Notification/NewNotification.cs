using DTO.Enums.Notification;

namespace DTO.Notification;

public record NewNotification
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public NotificationType Type { get; init; }
    public NotificationStatus Status { get; init; }
    public DateTime DateSent { get; init; }
    public string Data { get; init; } = null!;
}
