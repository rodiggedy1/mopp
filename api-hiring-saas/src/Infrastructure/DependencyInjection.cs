using Application.Common.Interfaces.Identity;
using Domain.Interfaces;
using Infrastructure.Caching;
using Infrastructure.Identity;
using Infrastructure.Identity.DependencyInjection;
using Infrastructure.MediaStorage;
using Infrastructure.MessageBroker;
using Infrastructure.Payments;
using Infrastructure.Search;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration,
        Assembly assembly)
    {
        services.AddIdentityModule(configuration);
        services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddSingleton<IEncryption, EncryptionService>();
        services.AddMessageBroker(assembly, true);
        services.AddMediaStorage(configuration);
        services.AddCaching(configuration);
        services.AddStripe(configuration.GetSection(StripeConfig.SectionName).Get<StripeConfig>()!);
        services.AddSearch();
        return services;
    }
}
