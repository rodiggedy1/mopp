using Domain.Entities.User;

namespace Domain.Events.Users;

public sealed class UserActivatedEvent : BaseEvent
{
    public UserActivatedEvent(ApplicationUser user)
    {
        User = user;
    }

    public ApplicationUser User { get; }
}
