using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Core.Config;
using Septem.Notifications.Jobs.Config;
using Septem.Util.Test.WorkerApp;
using Serilog;
using Serilog.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog((_, configuration) =>
    {
        configuration.WriteTo.Console();
    })
    .ConfigureServices(services =>
    {
        services.AddNotificationsForWorker(options =>
        {
            options.ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuted, LogLevel.Debug)));
            options.ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuting, LogLevel.Debug)));
            options.ConfigureWarnings(c => c.Log((CoreEventId.ContextInitialized, LogLevel.Debug)));
            options.UseLoggerFactory(new SerilogLoggerFactory());
            options.UseNpgsql("Server=localhost;Port=5432;Uid=postgres;Pwd=Qwerty1;Database=NotificationTestDbIndexed", b => b.MigrationsAssembly("Septem.Notifications.Core"));
        }, smsOptions =>
        {
            smsOptions.ApiEndpoint = "http://api.msm.az/sendsms";
            smsOptions.UserName = "novbemapi";
            smsOptions.Password = "GHf31doK";
            smsOptions.SenderName = "novbem.az";
        }, emailOptions =>
        {
            emailOptions.MailFromLogin = "irasim96@mail.ru";
            emailOptions.MailFromName = "Rasim Ismatulin";
            emailOptions.MailFromPassword = "qbHCjz4kS4qWv4Cb1XF3";
            emailOptions.SmtpHost = "smtp.mail.ru";
            emailOptions.SmtpPort = 587;
        }, fcmOptions =>
        {
            fcmOptions.AddInstance(File.ReadAllText(@"D:\Projects\Septem\RBIS\rbis.mobile.api\Rbis.Forum.Api.Notifications.Worker.WindowsService\rbis-mobile-app-fb008-firebase-adminsdk-ocio6-481596b623.json"));
        });
        services.AddNotificationJobs();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
