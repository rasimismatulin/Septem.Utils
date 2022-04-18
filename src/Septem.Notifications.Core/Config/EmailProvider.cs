namespace Septem.Notifications.Core.Config;

public class EmailProvider
{
    public string MailFromLogin { get; set; }
    public string MailFromName { get; set; }
    public string MailFromPassword { get; set; }

    public int SmtpPort { get; set; }
    public string SmtpHost { get; set; }

    public EmailProvider(string mailFromLogin, string mailFromName, string mailFromPassword, int smtpPort, string smtpHost)
    {
        MailFromLogin = mailFromLogin;
        MailFromName = mailFromName;
        MailFromPassword = mailFromPassword;
        SmtpPort = smtpPort;
        SmtpHost = smtpHost;
    }
}