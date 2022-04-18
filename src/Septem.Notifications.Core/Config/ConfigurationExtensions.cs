using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Infrastructure;
using Septem.Notifications.Core.Infrastructure.Repositories;
using Septem.Notifications.Core.Services;
using Septem.Notifications.Core.Services.Sender;
using Septem.Notifications.Core.Services.Token;

namespace Septem.Notifications.Core.Config;

public static class ConfigurationExtensions
{
    public static void AddNotifications(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsBuilder)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationTokenRepository, NotificationTokenRepository>();
        services.AddScoped<INotificationTokenService, NotificationTokenService>();
        services.AddScoped<INotificationMessageRepository, NotificationMessageRepository>();
        services.AddScoped<INotificationMessageHistoryService, NotificationMessageHistoryService>();

        services.AddDbContext<NotificationDbContext>(optionsBuilder);
    }


    public static void AddNotificationsForWorker(this IServiceCollection services,
        Action<DbContextOptionsBuilder> dbConfiguration,
        Action<SmsOptionBuilder> smsConfiguration = null,
        Action<EmailOptionBuilder> emailConfiguration = null,
        Action<FcmConfigurationBuilder> fcmConfiguration = null)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationTokenRepository, NotificationTokenRepository>();
        services.AddScoped<INotificationMessageRepository, NotificationMessageRepository>();

        services.AddScoped<INotificationMessageService, NotificationMessageService>();


        services.AddScoped<NotificationSenderServiceProvider>();
        services.AddScoped<NotificationTokenServiceProvider>();

        services.AddScoped<SmsNotificationTokenService>();
        services.AddScoped<EmailNotificationTokenService>();
        services.AddScoped<FcmNotificationTokenService>();
        services.AddScoped<FcmOrSmsNotificationTokenService>();

        services.AddScoped<SmsNotificationSenderService>();
        services.AddScoped<EmailNotificationSenderService>();
        services.AddScoped<FcmNotificationSenderService>();

        services.AddDbContext<NotificationDbContext>(dbConfiguration);

        var smsBuilder = new SmsOptionBuilder();
        smsConfiguration?.Invoke(smsBuilder);
        smsBuilder.Build();

        var emailBuilder = new EmailOptionBuilder();
        emailConfiguration?.Invoke(emailBuilder);
        emailBuilder.Build();

        var fcmBuilder = new FcmConfigurationBuilder();
        fcmConfiguration?.Invoke(fcmBuilder);
        fcmBuilder.Build();
    }


    public static void UseNotifications(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();

        if (scope != null)
        {
            scope.ServiceProvider.GetRequiredService<NotificationDbContext>().Database.Migrate();
            using var requiredService = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            requiredService.Database.Migrate();
        }
    }
}