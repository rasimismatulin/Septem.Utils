using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Septem.Notifications.Core;
using Septem.Notifications.Jobs.Config;
using Septem.Notifications.Jobs.JobExecution;

namespace Septem.Notifications.Jobs;

[DisallowConcurrentExecution]
internal class ReceiverCreateInitializationJob : IJob
{
    private readonly ITaskExecuteStrategyHandler _taskExecuteStrategyHandler;
    private readonly INotificationMessageService _notificationMessageService;

    public ReceiverCreateInitializationJob(ITaskExecuteStrategyHandler taskExecuteStrategyHandler, INotificationMessageService notificationMessageService)
    {
        _taskExecuteStrategyHandler = taskExecuteStrategyHandler;
        _notificationMessageService = notificationMessageService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (_taskExecuteStrategyHandler.CanHandle)
        {
            var notifications = await _notificationMessageService.GetPendingNotificationsUidAsync(JobOptionsBuilder.ReceiverCreateJobBatchSize, context.CancellationToken);

            if (notifications.Any())
            {
                var tasks = notifications.Select(x => new Func<IServiceProvider, Task>((serviceProvider) => CreteReceivers(serviceProvider, x))).ToList();
                await _taskExecuteStrategyHandler.Handle(tasks);
            }
        }
    }

    private async Task CreteReceivers(IServiceProvider serviceProvider, Guid uid)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var instance = scope.ServiceProvider.GetRequiredService<INotificationMessageService>();
        await instance.CreateNotificationReceiversAsync(uid);
    }
}