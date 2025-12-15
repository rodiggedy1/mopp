using Application.Common.Services;
using Infrastructure.Authentication.Handlers;
using Infrastructure.Common.Extensions;
using Infrastructure.Identity.Configuration;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Infrastructure.ApiClient;

public static class ApiClientInjection
{
    public static IServiceCollection AddApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfigurationBoundOptions<WorkerAuthConfig>(WorkerAuthConfig.SectionName);

        services.AddTransient<WorkerAuthTokenHandler>();

        var config = configuration.GetSection(WorkerAuthConfig.SectionName).Get<WorkerAuthConfig>();

        var apiClientInterfaceType = typeof(IApiClient);
        var assembly = apiClientInterfaceType.Assembly;

        var apiClientTypes = assembly.GetTypes()
            .Where(t => t.IsInterface && apiClientInterfaceType.IsAssignableFrom(t) && t != apiClientInterfaceType)
            .ToList();

        // Register each IApiClient implementation
        foreach (var apiClientType in apiClientTypes)
        {
            services.AddRefitClient(apiClientType)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(config!.ValidAudience);
                })
                .AddHttpMessageHandler<WorkerAuthTokenHandler>();
        }

        services.AddSingleton<IApiService, ApiService>();
        return services;
    }
}
