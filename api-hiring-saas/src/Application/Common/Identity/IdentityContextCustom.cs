using Application.Identity;

namespace Application.Common.Identity;

public record IdentityContextCustom(IUserInfo CurrentUser) : IIdentityContext;