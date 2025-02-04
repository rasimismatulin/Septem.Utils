using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Septem.Notifications.Core.Services.Sender.Email;

namespace Septem.Notifications.Core.Config;

public class EmailOptionBuilder
{
    private readonly IServiceCollection _services;
    internal static string MailFromLogin { get; set; }
    internal static string MailFromName { get; set; }
    internal static string MailFromPassword { get; set; }
    internal static int SmtpPort { get; set; }
    internal static string SmtpHost { get; set; }

    public EmailOptionBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public EmailOptionBuilder Default()
    {
        _services.AddTransient<EmailNotificationSenderService>();
        return this;
    }

    public EmailOptionBuilder ReadFromConfiguration(IConfigurationSection configuration)
    {
        MailFromLogin = configuration["MailFromLogin"];
        MailFromName = configuration["MailFromName"];
        MailFromPassword = configuration["MailFromPassword"];
        SmtpPort = int.Parse(configuration["SmtpPort"]!);
        SmtpHost = configuration["SmtpHost"];
        return this;
    }
}