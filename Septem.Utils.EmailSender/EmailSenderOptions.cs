
namespace Septem.Utils.EmailSender;

public class EmailSenderOptions
{
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpSslPort { get; set; }
    public string SmtpSender { get; set; }

    public string Receiver { get; set; }

    public string SmtpUser { get; set; }
    public string SmtpPassword { get; set; }
    public string Subject { get; set; }

    public EmailLevel MinLevel { get; set; }

    public static EmailSenderOptions Default => new()
    {
        MinLevel = EmailLevel.Warning,
    };
}