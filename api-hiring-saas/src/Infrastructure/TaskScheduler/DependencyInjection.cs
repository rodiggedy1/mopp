using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Infrastructure.TaskScheduler;

public static class DependencyInjection
{
    public static IServiceCollection AddTaskScheduler(this IServiceCollection services, Assembly assembly)
    {
        var tasks = assembly.GetExportedTypes()
                            .Where(x => !x.IsAbstract && typeof(ScheduledTaskBase).IsAssignableFrom(x)).Distinct();

        foreach (var type in tasks)
        {
            services.AddSingleton(typeof(IHostedService), type);
        }

        return services;
    }
}
