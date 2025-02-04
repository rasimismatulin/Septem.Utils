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

internal class ReceiverCreateInitializationJob : BaseHostedService
{
    public ReceiverCreateInitializationJob(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : 
        base(serviceProvider, loggerFactory)
    {

    }

    protected override async Task ExecuteInternalAsync(CancellationToken cancellationToken)
    {
        var taskExecuteStrategyHandler = ServiceProvider.GetRequiredService<ITaskExecuteStrategyHandler>();
        if (taskExecuteStrategyHandler.CanHandle)
        {
            var notificationMessageService = ServiceProvider.GetRequiredService<INotificationMessageService>();
            var notifications = await notificationMessageService.GetPendingNotificationsUidAsync(JobOptionsBuilder.ReceiverCreateJobBatchSize, cancellationToken);

            if (notifications.Any())
            {
                var tasks = notifications.Select(x => new Func<IServiceProvider, Task>((serviceProvider) => CreteReceivers(serviceProvider, x))).ToList();
                await taskExecuteStrategyHandler.Handle(tasks);
            }
        }
    }

    private async Task CreteReceivers(IServiceProvider serviceProvider, Guid uid)
    {
        var instance = serviceProvider.GetRequiredService<INotificationMessageService>();
        await instance.CreateNotificationReceiversAsync(uid);
    }
}