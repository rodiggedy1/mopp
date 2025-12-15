using Application.Common.Payments;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Payments;

public static class DependencyInjection
{
    public static void AddStripe(this IServiceCollection services, StripeConfig config)
    {
        services.AddSingleton<IPaymentProcessor>(serviceProvider => 
            new StripePaymentService(
                config, 
                serviceProvider.GetRequiredService<ILogger<StripePaymentService>>(),
                serviceProvider.GetRequiredService<IDateTime>()));
    }
}
