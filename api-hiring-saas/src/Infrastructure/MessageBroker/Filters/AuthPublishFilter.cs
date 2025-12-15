using Application.Common.Extensions;
using Application.Features.Authentication.Core;
using Infrastructure.MessageBroker.Extensions;
using MassTransit;

namespace Infrastructure.MessageBroker.Filters;

public class AuthPublishFilter<T> : IFilter<PublishContext<T>>
where T : class
{
    private readonly IAuthTokenProvider _authTokenProvider;

    public AuthPublishFilter(IAuthTokenProvider authTokenProvider)
    {
        _authTokenProvider = authTokenProvider;
    }

    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        var accessToken = await _authTokenProvider.GetAccessToken();
        if (accessToken.HasValue())
        {
            context.SetAccessToken(accessToken);
        }

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}
