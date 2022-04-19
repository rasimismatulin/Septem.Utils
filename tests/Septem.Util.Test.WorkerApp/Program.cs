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
            options.UseNpgsql("connectionString", b => b.MigrationsAssembly("Septem.Notifications.Core"));
        }, smsOptions =>
        {
            smsOptions.ApiEndpoint = "mytestApi";
            smsOptions.UserName = "mytestUser";
            smsOptions.Password = "mytestPasword";
            smsOptions.SenderName = "mytestSender";
        }, emailOptions =>
        {
            emailOptions.MailFromLogin = "mytestLogin";
            emailOptions.MailFromName = "myTestName";
            emailOptions.MailFromPassword = "MailFrom";
            emailOptions.SmtpHost = "mailHost";
            emailOptions.SmtpPort = 999;
        }, fcmOptions =>
        {
            fcmOptions.AddInstance("privateSecret");
        });
        services.AddNotificationJobs();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
