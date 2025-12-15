using Application.Identity;
using System.Security.Claims;

namespace Infrastructure.Identity.Models;

public class UserInfo : IUserInfo
{
    private readonly ClaimsPrincipal _claimsPrincipal;

    public UserInfo(ClaimsPrincipal claimsPrincipal)
    {
        _claimsPrincipal = claimsPrincipal;
    }

    public int Id => int.Parse(_claimsPrincipal.FindFirstValue(Configuration.IdentityConstants.Claims.Id)!);
    public string UserName => _claimsPrincipal.FindFirstValue(Configuration.IdentityConstants.Claims.UserName)!;
    public bool? IsAdmin
    {
        get
        {
            var claimValue = _claimsPrincipal.FindFirstValue(Configuration.IdentityConstants.Claims.IsAdmin);
            if (bool.TryParse(claimValue, out var result))
            {
                return result;
            }
            return null;
        }
    }
}
