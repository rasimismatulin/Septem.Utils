using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Infrastructure;
using Septem.Notifications.Core.Services;
using Septem.Notifications.Core.Services.Sender;
using Septem.Notifications.Core.Services.Sender.Sms;
using Septem.Notifications.Core.Services.Token;

namespace Septem.Notifications.Core.Config;

public class NotificationsCoreConfiguration
{
    private readonly IServiceCollection _services;

    public NotificationsCoreConfiguration(IServiceCollection services)
    {
        _services = services;
    }

    public NotificationsCoreConfiguration Fcm(Action<FcmConfigurationBuilder> config)
    {
        var builder = new FcmConfigurationBuilder(_services);
        config(builder);
        return this;
    }

    public NotificationsCoreConfiguration Email(Action<EmailOptionBuilder> config)
    {
        var builder = new EmailOptionBuilder(_services);
        config(builder);
        return this;
    }

    public NotificationsCoreConfiguration Sms(Action<SmsOptionBuilder> config)
    {
        var builder = new SmsOptionBuilder(_services);
        config(builder);
        return this;
    }
    public NotificationsCoreConfiguration Database(Action<IServiceProvider, DbContextOptionsBuilder, string> dbConfiguration)
    {
        _services.AddDbContext<NotificationDbContext>((provider, optionsBuilder) =>
                dbConfiguration(provider, optionsBuilder, typeof(ConfigurationExtensions).Assembly.FullName),
            ServiceLifetime.Transient, ServiceLifetime.Singleton);
        return this;
    }
}

public static class ConfigurationExtensions
{
    public static IServiceCollection AddNotificationCore(this IServiceCollection services, Action<NotificationsCoreConfiguration> configure)
    {
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<INotificationTokenService, NotificationTokenService>();
        services.AddTransient<INotificationMessageHistoryService, NotificationMessageHistoryService>();

        services.AddTransient<INotificationMessageService, NotificationMessageService>();

        services.AddTransient<NotificationSenderServiceProvider>();
        services.AddTransient<NotificationTokenServiceProvider>();

        services.AddTransient<SmsNotificationTokenService>();
        services.AddTransient<EmailNotificationTokenService>();
        services.AddTransient<FcmNotificationTokenService>();
        services.AddTransient<FcmOrSmsNotificationTokenService>();


        services.AddHttpClient(nameof(SmsNotificationSenderService));

        configure(new NotificationsCoreConfiguration(services));
        return services;
    }



    public static async Task UseNotificationCore(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.MigrateAsync();
    }

    public static async Task UseNotificationCore(this IHost app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        await scope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.MigrateAsync();
    }
}