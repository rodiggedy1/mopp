using Microsoft.Extensions.Configuration;

namespace Application.Common.Extensions;

public static class ConfigurationExtensions
{
    public static T GetInstance<T>(this IConfiguration configuration)
    {
        return configuration.GetSection(typeof(T).Name).Get<T>();
    }

    public static string GetStringValue(this IConfiguration configuration, string key)
    {
        return configuration.GetSection(key)?.Value;
    }
}
