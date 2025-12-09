using Application.Identity;
using Domain.Entities.User;

namespace Application.Common.Identity;

public class DefaultUserInfo : IUserInfo
{
    private readonly ApplicationUser _user;

    public DefaultUserInfo(ApplicationUser user)
    {
        _user = user;
    }

    public int Id => _user.Id;
    public string UserName => _user.UserName!;
    public bool? IsAdmin => default;
}
