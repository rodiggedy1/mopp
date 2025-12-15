using Application.Common.Caching;
using Application.Common.Localization;
using Application.Common.MessageBroker;
using DTO.MessageBroker.Messages.System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Infrastructure.Caching;

public class PrivateSetterContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (!property.Writable && member is PropertyInfo propertyInfo)
        {
            property.Writable = CanSet(propertyInfo);
        }

        return property;
    }

    private bool CanSet(PropertyInfo propertyInfo)
    {
        return propertyInfo.GetSetMethod(true) != null;
    }
}

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<CacheService> _logger;
    private readonly JsonSerializerSettings _serializerSettings;
    private readonly IMessagePublisher _messagePublisher;

    public CacheService(
        IDistributedCache distributedCache, 
        ILocalizationService localizationService,
        ILogger<CacheService> logger,
        IMessagePublisher messagePublisher)
    {
        _distributedCache = distributedCache;
        _localizationService = localizationService;
        _logger = logger;
        _messagePublisher = messagePublisher;
        _serializerSettings =  new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterContractResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var language in _localizationService.AvailableLanguages)
            {
                string keyWithCulture = $"{key}-{language.Code}";
                await _distributedCache.RemoveAsync(keyWithCulture, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to remove key from cache: {Key}. This might be due to Redis being in read-only mode.", key);
            await _messagePublisher.PublishAsync(new RestartRedisMessage(ex.ToString()));
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        if (_localizationService.CurrentLanguage != null)
        {
            key = $"{key}-{_localizationService.CurrentLanguage.Code}";
        }

        var cachedString = await _distributedCache.GetStringAsync(
                            key,
                            cancellationToken);

        if (!string.IsNullOrEmpty(cachedString))
        {
            return JsonConvert.DeserializeObject<T>(cachedString, _serializerSettings)!;
        }
        return default;
    }

    public async Task AddAsync<T>(string key, T value, CancellationToken cancellationToken)
    {
        try
        {
            if (_localizationService.CurrentLanguage != null)
            {
                key = $"{key}-{_localizationService.CurrentLanguage.Code}";
            }

            await _distributedCache.SetStringAsync(
                    key,
                    JsonConvert.SerializeObject(value, _serializerSettings),
                    cancellationToken);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Unable to add key to cache: {Key}. This might be due to Redis being in read-only mode.", key);
            await _messagePublisher.PublishAsync(new RestartRedisMessage(ex.ToString()));
        }
    }

    public async Task ClearCache(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Clearing cache on startup...");
        var cacheKeys = typeof(CacheKeys)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.IsLiteral && !field.IsInitOnly)  // Ensure it's a constant
            .Select(field => field.GetValue(null)!.ToString())
            .ToList();

        foreach (var key in cacheKeys)
        {
            try
            {
                await RemoveAsync(key!, cancellationToken);
                _logger.LogDebug("Cleared cache for key: {key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cache for key: {key}", key);
            }

        }
    }
}
public class PrivateResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty prop = base.CreateProperty(member, memberSerialization);

        if (!prop.Writable)
        {
            var property = member as PropertyInfo;
            bool hasPrivateSetter = property?.GetGetMethod(true) != null;
            prop.Writable = hasPrivateSetter;
        }
        return prop;
    }
}

public class CacheClearingService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CacheClearingService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Create a scope to resolve the scoped service (ICacheService)
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            // Call ClearCache when the application starts
            await cacheService.ClearCache(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}