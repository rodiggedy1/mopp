using Application.Common.Extensions;
using Application.Identity;
using Infrastructure.Identity.Models;
using Infrastructure.Identity;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MessageBroker.Extensions;
public class AuthConsumeFilter<T> : IFilter<ConsumeContext<T>>
where T : class
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityContextAccessor _identityContextAccessor;
    private readonly ILogger<AuthConsumeFilter<T>> _logger;

    public AuthConsumeFilter(
        IJwtTokenService jwtTokenService,
        IIdentityContextAccessor identityContextAccessor,
        ILogger<AuthConsumeFilter<T>> logger)
    {
        _jwtTokenService = jwtTokenService;
        _identityContextAccessor = identityContextAccessor;
        _logger = logger;
    }

    public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var accessToken = context.GetAccessToken();
        if (accessToken.IsEmpty())
        {
            _logger.LogDebug("Access token is empty in AuthConsumeFilter {0}", accessToken);
            return next.Send(context);
        }

        var claimsPrincipal = _jwtTokenService.GetPrincipalFromToken(accessToken, true);
        if (claimsPrincipal == null)
        {
            return next.Send(context);
        }

        _identityContextAccessor.IdentityContext = new IdentityContextCustom(new UserInfo(claimsPrincipal));
        return next.Send(context);
    }

    public void Probe(ProbeContext context) { }
}