using Domain.Entities.User;

namespace Domain.Events.Users;

public sealed class UserSuspensionRemovedEvent : BaseEvent
{
    public UserSuspensionRemovedEvent(ApplicationUser user)
    {
        User = user;
    }

    public ApplicationUser User { get; }
}
