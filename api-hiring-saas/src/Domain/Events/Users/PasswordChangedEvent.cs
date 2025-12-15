using Domain.Entities.User;

namespace Domain.Events.Users;

public sealed class PasswordChangedEvent : BaseEvent
{
    public PasswordChangedEvent(ApplicationUser user, string oldPassword, string ipAddress)
    {
        User = user;
        OldPassword = oldPassword;
        IpAddress = ipAddress;
    }

    public ApplicationUser User { get; set; }
    public string OldPassword { get; }
    public string IpAddress { get; }
}
