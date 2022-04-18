using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Config;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Services.Sender;

internal class EmailNotificationSenderService : INotificationSenderService
{
    public Task<SentStatus> SendAsync(Notification notification, NotificationMessageEntity message, NotificationTokenEntity token, CancellationToken cancellationToken)
    {
        try
        {
            using var mail = new MailMessage();
            mail.From = new MailAddress(EmailOptionBuilder.EmailOptionProvider.MailFromLogin, EmailOptionBuilder.EmailOptionProvider.MailFromName);
            mail.To.Add(new MailAddress(token.Token));
            mail.Subject = message.Title;
            mail.Body = message.Payload;

            using var client = new SmtpClient();
            client.Host = EmailOptionBuilder.EmailOptionProvider.SmtpHost;
            client.Port = EmailOptionBuilder.EmailOptionProvider.SmtpPort;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(EmailOptionBuilder.EmailOptionProvider.MailFromLogin, EmailOptionBuilder.EmailOptionProvider.MailFromPassword); 
            client.Send(mail);

            return Task.FromResult(SentStatus.Success("No response"));
        }
        catch (Exception e)
        {
            return Task.FromResult(SentStatus.Fail(e));
        }
    }
}