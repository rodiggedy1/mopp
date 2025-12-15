using Domain.Entities.User;

namespace Domain.Events.Users;

public sealed class UserSuspendedEvent : BaseEvent
{
    public UserSuspendedEvent(ApplicationUser user)
    {
        User = user;
    }

    public ApplicationUser User { get; }
}
