using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Core;
using Septem.Notifications.Worker.Config;
using Septem.Notifications.Worker.JobExecution;

namespace Septem.Notifications.Worker.HostedServices;

internal class MessageCreateInitializationJob : BaseHostedService
{
    public MessageCreateInitializationJob(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        : base(serviceProvider, loggerFactory)
    {
    }

    protected override async Task ExecuteInternalAsync(CancellationToken cancellationToken)
    {
        var taskExecuteStrategyHandler = ServiceProvider.GetRequiredService<ITaskExecuteStrategyHandler>();
        if (taskExecuteStrategyHandler.CanHandle)
        {
            var notificationMessageService = ServiceProvider.GetRequiredService<INotificationMessageService>();
            var notifications = await notificationMessageService.GetNextNotificationsUidAsync(JobOptionsBuilder.MessageCreateJobBatchSize, cancellationToken);
            if (notifications.Any())
            {
                Logger.LogInformation($"Build Create messages tasks. Count: {notifications.Count}");
                var tasks = notifications.Select(x => new Func<IServiceProvider, Task>(sp => CreteMessages(sp, x))).ToList();
                await taskExecuteStrategyHandler.Handle(tasks);
            }
        }
    }

    private async Task CreteMessages(IServiceProvider serviceProvider, Guid uid)
    {
        var instance = serviceProvider.GetRequiredService<INotificationMessageService>();
        await instance.CreateNotificationMessagesAsync(uid);
    }
}