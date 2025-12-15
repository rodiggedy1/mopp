using Domain.Entities.User;

namespace Domain.Events.Users;

public sealed class UserDeactivatedEvent : BaseEvent
{
    public UserDeactivatedEvent(ApplicationUser user)
    {
        User = user;
    }

    public ApplicationUser User { get; }
}
