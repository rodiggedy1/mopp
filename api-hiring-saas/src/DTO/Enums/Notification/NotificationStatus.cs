using DTO.Attributes;

namespace DTO.Enums.Notification;

public enum NotificationStatus
{
    [LocalizationKey("enum.notificationStatus.read")]
    Read = 1,
    [LocalizationKey("enum.notificationStatus.unread")]
    Unread
}
