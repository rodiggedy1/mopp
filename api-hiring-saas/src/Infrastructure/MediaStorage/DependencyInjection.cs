using Application.Common.MediaStorage;
using Application.Common.MediaStorage.Interfaces;
using Domain.Interfaces;
using Infrastructure.Common.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MediaStorage;

public static class DependencyInjection
{
    public static IServiceCollection AddMediaStorage(this IServiceCollection services, IConfiguration configuration)
    {
        SslConfigurationHelper.ConfigureGlobalSslSettings();
        
        services.AddScoped<MediaEntityResolver>();
        services.AddSingleton<IFileSystemProvider, FileSystemProvider>();
        services.AddSingleton<IMediaStorageHelper, MediaStorageHelper>();
        services.AddConfigurationBoundOptions<MediaConfig>(MediaConfig.SectionName);

        services.AddScoped<IMediaEntityResolver, MediaEntityResolver>();

        var config = configuration.GetSection(MediaConfig.SectionName).Get<MediaConfig>();

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = config!.MaxFileSizeInKb * 1024;
        });

        if (config!.StorageProviderConfigs.All(cfg => cfg == null))
        {
            throw new ApplicationException("Could not resolve media storage provider");
        }

        switch (config.StorageProviderType)
        {
            case MediaStorageProviderType.FileSystem:
                RegisterFileSystemMediaStorage(services);
                break;
            case MediaStorageProviderType.CloudflareR2:
                RegisterCloudflareR2MediaStorageWithFallback(services, config);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return services;
    }

    private static void RegisterFileSystemMediaStorage(IServiceCollection services)
    {
        services.AddSingleton<IMediaStorage, MediaFileSystemStorage>();
    }

    private static void RegisterCloudflareR2MediaStorageWithFallback(IServiceCollection services, MediaConfig config)
    {
        Console.WriteLine("DependencyInjection: Registering CloudflareR2 storage with local fallback support");
        
        services.AddSingleton<IMediaStorage>(provider =>
        {
            var configOptions = Microsoft.Extensions.Options.Options.Create(config);
            var mediaStorageHelper = provider.GetRequiredService<IMediaStorageHelper>();
            var fileSystemProvider = provider.GetRequiredService<IFileSystemProvider>();
            
            return new CloudflareR2Storage(configOptions, mediaStorageHelper, fileSystemProvider);
        });
        
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(3000);
                Console.WriteLine("DependencyInjection: CloudflareR2 with fallback registered successfully");
                
                var testResult = await SslConfigurationHelper.TestCloudflareR2Connectivity(config.CloudflareR2!.AccountId);
                if (!testResult)
                {
                    Console.WriteLine("DependencyInjection: R2 connectivity test failed - fallback will be used");
                }
                else
                {
                    Console.WriteLine("DependencyInjection: R2 connectivity test passed - primary storage ready");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DependencyInjection: R2 connectivity test error: {ex.Message}");
            }
        });
    }
}
