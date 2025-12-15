using Infrastructure.MessageBroker.Filters;
using MassTransit;

namespace Infrastructure.MessageBroker.Extensions;

public static class MessageBrokeFilterConfiguratorExtensions
{
    public static IRabbitMqBusFactoryConfigurator UseAuthFilters(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context)
    {
        cfg.UsePublishFilter(typeof(AuthPublishFilter<>), context);
        cfg.UseConsumeFilter(typeof(AuthConsumeFilter<>), context);

        return cfg;
    }
}
