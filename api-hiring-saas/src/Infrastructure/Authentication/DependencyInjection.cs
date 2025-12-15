using Application.Features.Authentication;
using Application.Features.Authentication.Core;
using Infrastructure.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddHttpContextAuthProvider(this IServiceCollection services)
    {
        services.TryAddScoped<IAuthTokenProvider, HttpContextAuthTokenProvider>();
        return services;
    }

    public static IServiceCollection AddConsumeContextAuthProvider(this IServiceCollection services)
    {
        //services.TryAddScoped<IAuthTokenProvider, ConsumeContextAuthTokenProvider>();
        services.TryAddSingleton<IAuthTokenProvider, ConsumeContextAuthTokenProvider>();
        return services;
    }
}
