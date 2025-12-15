using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Infrastructure.Logging;

public static class LoggingExtensions
{
    public static IHostBuilder AddLogging(this IHostBuilder builder)
    {
        builder.UseSerilog((context, configuration) => BuildLogging(configuration, context));
        return builder;
    }

    private static LoggerConfiguration BuildLogging(
        this LoggerConfiguration loggerConfiguration,
        HostBuilderContext context)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var elasticsearchConfig = context.Configuration
            .GetSection("Serilog:WriteTo")
            .GetChildren()
            .FirstOrDefault(x => x.GetValue<string>("Name") == "Elasticsearch")
            ?.GetSection("Args");

        loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Destructure.ToMaximumDepth(1)
                .Enrich.WithProperty("Environment", environmentName)
                .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                .WriteTo.Console();

        // Logging to file
        if (environmentName == "Development")
        {
            loggerConfiguration
                .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "Logs", "log-.txt"), rollingInterval: RollingInterval.Day);
        }

        var elasticSearchUrl = elasticsearchConfig?.GetValue<string>("nodeUris");

        // Logging to Elasticsearch
        if (!string.IsNullOrEmpty(elasticSearchUrl))
        {
            loggerConfiguration.WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri(elasticSearchUrl))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    IndexFormat = elasticsearchConfig!.GetValue<string>("indexFormat"),
                    ModifyConnectionSettings = cfg =>
                    {
                        var userName = elasticsearchConfig!.GetValue<string>("userName");
                        var password = elasticsearchConfig!.GetValue<string>("password");
                        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                        {
                            cfg = cfg.BasicAuthentication(userName, password);
                        }

                        return cfg.EnableApiVersioningHeader();
                    }
                });
        }

        return loggerConfiguration;
    }
}
