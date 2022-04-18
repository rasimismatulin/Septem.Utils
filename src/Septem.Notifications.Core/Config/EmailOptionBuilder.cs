namespace Septem.Notifications.Core.Config;

public class EmailOptionBuilder
{
    public static EmailProvider EmailOptionProvider;


    public string MailFromLogin { get; set; }
    public string MailFromName { get; set; }
    public string MailFromPassword { get; set; }

    public int SmtpPort { get; set; }
    public string SmtpHost { get; set; }

    public void Build()
    {
        EmailOptionProvider = new EmailProvider(MailFromLogin, MailFromName, MailFromPassword, SmtpPort, SmtpHost);
    }
}