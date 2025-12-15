using DTO.Attributes;

namespace DTO.Enums.User;

public enum UserStatus
{
    [LocalizationKey("enum.userStatus.deleted")]
    Deleted = -1,
    [LocalizationKey("enum.userStatus.active")]
    Active = 1,
    [LocalizationKey("enum.userStatus.deactivated")]
    Deactivated = 2,
    [LocalizationKey("enum.userStatus.suspended")]
    Suspended = 3,
    [LocalizationKey("enum.userStatus.awaitingConfirmation")]
    AwaitingConfirmation = 4,
    [LocalizationKey("enum.userStatus.awaitingInvitation")]
    AwaitingInvitation = 5
}
