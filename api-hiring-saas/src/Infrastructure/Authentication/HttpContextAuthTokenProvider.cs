using Application.Features.Authentication.Core;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Authentication;

public class HttpContextAuthTokenProvider : IAuthTokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextAuthTokenProvider(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string?> GetAccessToken()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            return default;
        }

        var accessToken = _httpContextAccessor.HttpContext.Request.Headers[AuthConstants.AccessTokenName];
        return accessToken;
    }
}
