using System;
using Microsoft.Extensions.DependencyInjection;
using Septem.Notifications.Worker.HostedServices;
using Septem.Notifications.Worker.JobExecution;

namespace Septem.Notifications.Worker.Config;

public static class ConfigurationExtensions
{
    public static void AddNotificationJobs(this IServiceCollection services, Action<JobOptionsBuilder> jobConfigurationBuilder = null)
    {
        services.AddHostedService<ReceiverCreateInitializationJob>();
        services.AddHostedService<MessageCreateInitializationJob>();
        services.AddHostedService<MessageSendInitializationJob>();
        services.AddHostedService<ConcurrentQueueDispatcherJob>();

        services.AddSingleton<ConcurrentQueueDispatcher>();

        var builder = new JobOptionsBuilder();
        jobConfigurationBuilder?.Invoke(builder);
        builder.Build(services);
    }
}