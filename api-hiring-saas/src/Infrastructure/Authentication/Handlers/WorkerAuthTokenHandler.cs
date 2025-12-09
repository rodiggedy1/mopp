using Application.Identity;

namespace Infrastructure.Authentication.Handlers;

public class WorkerAuthTokenHandler : DelegatingHandler
{
    private readonly IJwtTokenService _jwtTokenService;

    public WorkerAuthTokenHandler(IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _jwtTokenService.GenerateWorkerToken();

        request.Headers.Add("WorkerAuth", $"Bearer {token}");

        return await base.SendAsync(request, cancellationToken);
    }
}
