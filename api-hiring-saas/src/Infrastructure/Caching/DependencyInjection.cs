using Application.Common.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Caching;

public static class DependencyInjection
{
    public static void AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });
        services.AddScoped<ICacheService, CacheService>();

        // Service for clearing the cache at startup
        services.AddSingleton<IHostedService, CacheClearingService>();
    }
}
