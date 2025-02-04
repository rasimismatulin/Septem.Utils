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

internal class MessageSendInitializationJob : BaseHostedService
{

    public MessageSendInitializationJob(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) 
        : base(serviceProvider, loggerFactory)
    {
    }
    protected override async Task ExecuteInternalAsync(CancellationToken cancellationToken)
    {
        var taskExecuteStrategyHandler = ServiceProvider.GetRequiredService<ITaskExecuteStrategyHandler>();
        if (taskExecuteStrategyHandler.CanHandle)
        {
            var notificationMessageService = ServiceProvider.GetRequiredService<INotificationMessageService>();
            var messages = await notificationMessageService.GetNextMessagesUidAsync(JobOptionsBuilder.MessageSendJobBatchSize, cancellationToken);

            if (messages.Any())
            {
                var tasks = messages.Select(x => new Func<IServiceProvider, Task>((serviceProvider) => SendMessages(serviceProvider, x))).ToList();
                await taskExecuteStrategyHandler.Handle(tasks);
            }
        }
    }

    private async Task SendMessages(IServiceProvider serviceProvider, Guid uid)
    {
        var instance = serviceProvider.GetRequiredService<INotificationMessageService>();
        await instance.SendMessagesAsync(uid);
    }
}