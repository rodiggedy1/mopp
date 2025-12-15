using Domain.Entities.User;

namespace Domain.Events.Users;

public sealed class UserCreatedEvent : BaseEvent
{
    public UserCreatedEvent(ApplicationUser user)
    {
        User = user;
    }

    public ApplicationUser User { get; }
}
