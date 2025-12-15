using Domain.Entities.User;

namespace Domain.Events.Users;

public sealed class UserEmailConfirmedEvent : BaseEvent
{
    public UserEmailConfirmedEvent(ApplicationUser user)
    {
        User = user;
    }

    public ApplicationUser User { get; }
}
