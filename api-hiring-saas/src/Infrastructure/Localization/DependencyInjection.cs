using Application.Common.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Localization;

public static class DependencyInjection
{
    public static void AddLocalization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ILocalizationManager, LocalizationManager>();
        services.AddSingleton<ILocalizationService, LocalizationService>();
    }
}
