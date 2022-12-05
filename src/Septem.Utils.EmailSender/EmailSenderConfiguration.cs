using Microsoft.Extensions.Configuration;
using System.Net.NetworkInformation;
using System;

namespace Septem.Utils.EmailSender;

public class EmailSenderConfiguration
{
    private readonly EmailSenderOptions _options;

    public EmailSenderConfiguration(EmailSenderOptions options)
    {
        _options = options;
    }
    public void Configuration(IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("EmailSender");
        _options.MinLevel = (EmailLevel)Enum.Parse(typeof(EmailLevel), section["MinLevel"], true);
        _options.SmtpHost = section["SmtpHost"];
        _options.SmtpPort = int.Parse(section["SmtpPort"]);
        _options.SmtpSslPort = section["SmtpSslPort"];
        _options.SmtpSender = section["SmtpSender"];
        _options.Receiver = section["Receiver"];
        _options.SmtpUser = section["SmtpUser"];
        _options.SmtpPassword = section["SmtpPassword"];
        _options.Subject = section["Subject"];
    }
}