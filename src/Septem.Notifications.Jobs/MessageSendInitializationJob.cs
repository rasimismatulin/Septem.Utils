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
internal class MessageSendInitializationJob : IJob
{
    private readonly ITaskExecuteStrategyHandler _taskExecuteStrategyHandler;
    private readonly INotificationMessageService _notificationMessageService;

    public MessageSendInitializationJob(ITaskExecuteStrategyHandler taskExecuteStrategyHandler, INotificationMessageService notificationMessageService)
    {
        _taskExecuteStrategyHandler = taskExecuteStrategyHandler;
        _notificationMessageService = notificationMessageService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (_taskExecuteStrategyHandler.CanHandle)
        {
            var messages = await _notificationMessageService.GetNextMessagesUidAsync(JobOptionsBuilder.MessageSendJobBatchSize, context.CancellationToken);

            if (messages.Any())
            {
                var tasks = messages.Select(x => new Func<IServiceProvider, Task>((serviceProvider) => SendMessages(serviceProvider, x))).ToList();
                await _taskExecuteStrategyHandler.Handle(tasks);
            }
        }
    }

    private async Task SendMessages(IServiceProvider serviceProvider, Guid uid)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var instance = scope.ServiceProvider.GetRequiredService<INotificationMessageService>();
        await instance.SendMessagesAsync(uid);
    }
}