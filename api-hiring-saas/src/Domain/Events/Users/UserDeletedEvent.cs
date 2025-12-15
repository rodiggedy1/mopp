using Domain.Entities.User;

namespace Domain.Events.Users;

public sealed class UserDeletedEvent : BaseEvent
{
    public UserDeletedEvent(ApplicationUser user)
    {
        User = user;
    }

    public ApplicationUser User { get; }
}
