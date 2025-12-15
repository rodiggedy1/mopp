using Application.Common.Search;
using Elasticsearch.Net;
using Infrastructure.Common.Extensions;
using Infrastructure.Search.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;

namespace Infrastructure.Search;

public static class DependencyInjection
{
    public static void AddSearch(this IServiceCollection services)
    {
        services.AddScoped(typeof(ISearchClient<>), typeof(ElasticSearchClient<>));
        services.AddSingleton<ISearchIndexProvider, SearchIndexProvider>();
        services.AddConfigurationBoundOptions<SearchConfig>(SearchConfig.SectionName);

        services.AddSingleton<IElasticClient>(
            provider =>
            {
                var elasticConfig = provider.GetRequiredService<IOptions<SearchConfig>>().Value;
                var connectionPool = new SingleNodeConnectionPool(new Uri(elasticConfig.Url));

                var connectionSettings = new ConnectionSettings(connectionPool)
                    .DefaultIndex(SearchIndex.Default)
                    .DisableDirectStreaming()
                    .PrettyJson();

                if (!string.IsNullOrEmpty(elasticConfig.Username) && !string.IsNullOrEmpty(elasticConfig.Password))
                {
                    connectionSettings.BasicAuthentication(elasticConfig.Username, elasticConfig.Password);
                }

                connectionSettings.RequestTimeout(elasticConfig.Timeout);

                return new ElasticClient(connectionSettings);
            });
    }
}
