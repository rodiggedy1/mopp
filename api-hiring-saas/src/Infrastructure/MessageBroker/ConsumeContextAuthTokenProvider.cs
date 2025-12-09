using Application.Common.Extensions;
using Application.Features.Authentication.Core;
using Infrastructure.MessageBroker.Extensions;

namespace Infrastructure.MessageBroker;

public class ConsumeContextAuthTokenProvider : IAuthTokenProvider
{
    private readonly IConsumeContextAccessor _consumeContextAccessor;

    public ConsumeContextAuthTokenProvider(IConsumeContextAccessor consumeContextAccessor)
    {
        _consumeContextAccessor = consumeContextAccessor;
    }

    public Task<string?> GetAccessToken()
    {
        var accessToken = _consumeContextAccessor.ConsumeContext?.GetAccessToken();
        return accessToken.AsTask();
    }
}