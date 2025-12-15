namespace Application.Features.Authentication.Core;

public interface IAuthTokenProvider
{
    Task<string?> GetAccessToken();
}