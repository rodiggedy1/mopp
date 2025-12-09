using Application.Common.Interfaces.Identity;
using Application.Identity;
using Domain.Entities.User;
using Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace Infrastructure.TaskScheduler;

public abstract class ScheduledTaskBase : IHostedService
{
    private readonly CancellationTokenSource _stoppingCancellationTokenSource;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CrontabSchedule _schedule;
    private Task _executingTask;
    private DateTime _nextRun;

    protected abstract string Schedule { get; }
    protected abstract string Name { get; }

    protected ILogger<ScheduledTaskBase> Logger { get; private set; }

    public ScheduledTaskBase(IServiceScopeFactory serviceScopeFactory)
    {
        _stoppingCancellationTokenSource = new CancellationTokenSource();
        _serviceScopeFactory = serviceScopeFactory;
        _schedule = CrontabSchedule.Parse(Schedule);
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
    }

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        _executingTask = ExecuteAsync(_stoppingCancellationTokenSource.Token);

        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null)
            return;

        try
        {
            // Signal cancellation to the executing method
            _stoppingCancellationTokenSource.Cancel();
        }
        finally
        {
            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            if (DateTime.Now > _nextRun)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    await Process(scope.ServiceProvider);
                }

                _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            }

            await Task.Delay(10000, stoppingToken);
        }
        while (!stoppingToken.IsCancellationRequested);
    }

    private async Task Process(IServiceProvider serviceProvider)
    {
        Logger = serviceProvider.GetService<ILogger<ScheduledTaskBase>>();
        Logger.LogInformation($"[{Name}] Job started.");

        try
        {
            var applicationUserManager = serviceProvider.GetService<IApplicationUserManager>();
            var administrator = await applicationUserManager!.GetByEmailAsync("administrator@localhost");

            if (administrator != null)
            {
                var identityContextSetter = serviceProvider.GetService<IIdentityContextAccessor>();
                identityContextSetter!.IdentityContext = new IdentityContextCustom(new ScheduledTaskUser(administrator));

                await Run(serviceProvider);

                identityContextSetter.IdentityContext = null;
            }

        }
        catch (Exception e)
        {
            Logger.LogError(e, $"[{Name}] Error during process.");
        }

        Logger.LogInformation($"[{Name}] Job finished.");
    }

    protected abstract Task Run(IServiceProvider serviceProvider);


    private class ScheduledTaskUser : IUserInfo
    {
        private readonly ApplicationUser _user;

        public ScheduledTaskUser(ApplicationUser user)
        {
            _user = user;
        }

        public int Id => _user.Id;
        public string UserName => _user.UserName!;
        public bool? IsAdmin => true;
    }
}