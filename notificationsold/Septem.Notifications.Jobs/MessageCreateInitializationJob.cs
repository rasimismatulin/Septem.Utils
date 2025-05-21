using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Septem.Notifications.Core;
using Septem.Notifications.Jobs.Config;
using Septem.Notifications.Jobs.JobExecution;

namespace Septem.Notifications.Jobs;

[DisallowConcurrentExecution]
internal class MessageCreateInitializationJob : IJob
{
    private readonly ITaskExecuteStrategyHandler _taskExecuteStrategyHandler;
    private readonly INotificationMessageService _notificationMessageService;
    private readonly ILogger _logger;

    public MessageCreateInitializationJob(ILoggerFactory loggerFactory, ITaskExecuteStrategyHandler taskExecuteStrategyHandler, INotificationMessageService notificationMessageService)
    {
        _logger = loggerFactory.CreateLogger<MessageCreateInitializationJob>();
        _taskExecuteStrategyHandler = taskExecuteStrategyHandler;
        _notificationMessageService = notificationMessageService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (_taskExecuteStrategyHandler.CanHandle)
        {
            var notifications = await _notificationMessageService.GetNextNotificationsUidAsync(JobOptionsBuilder.MessageCreateJobBatchSize, context.CancellationToken);
            if (notifications.Any())
            {
                _logger.LogInformation($"Build Create messages tasks. Count: {notifications.Count}");
                var tasks = notifications.Select(x => new Func<IServiceProvider, Task>(sp => CreteMessages(sp, x))).ToList();
                await _taskExecuteStrategyHandler.Handle(tasks);
            }
        }
    }

    private async Task CreteMessages(IServiceProvider serviceProvider, Guid uid)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var instance = scope.ServiceProvider.GetRequiredService<INotificationMessageService>();
        await instance.CreateNotificationMessagesAsync(uid);
    }
}