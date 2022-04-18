using System;
using Microsoft.Extensions.DependencyInjection;
using Septem.Notifications.Jobs.Factory;
using Septem.Notifications.Jobs.JobExecution;

namespace Septem.Notifications.Jobs.Config;

public static class ConfigurationExtensions
{
    public static void AddNotificationJobs(this IServiceCollection services, Action<JobOptionsBuilder> jobConfigurationBuilder = null)
    {
        services.AddScoped<ReceiverCreateInitializationJob>();
        services.AddScoped<MessageCreateInitializationJob>();
        services.AddScoped<MessageSendInitializationJob>();
        services.AddScoped<ConcurrentQueueDispatcherJob>();


        services.AddSingleton<DecoratorJobFactory>();
        services.AddSingleton<JobScheduler>();

        services.AddSingleton<ConcurrentQueueDispatcher>();

        var builder = new JobOptionsBuilder();
        jobConfigurationBuilder?.Invoke(builder);
        builder.Build(services);
    }
}