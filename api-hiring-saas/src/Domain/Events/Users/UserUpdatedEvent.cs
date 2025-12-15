using Domain.Entities.User;

namespace Domain.Events.Users;

public sealed class UserUpdatedEvent : BaseEvent
{
    public UserUpdatedEvent(ApplicationUser user)
    {
        User = user;
    }

    public ApplicationUser User { get; }
}
