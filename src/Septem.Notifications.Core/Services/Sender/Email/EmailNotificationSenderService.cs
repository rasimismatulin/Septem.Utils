using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Config;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Services.Sender.Email;

internal class EmailNotificationSenderService : INotificationSenderService
{
    public virtual async Task<SentStatus> SendAsync(Notification notification, NotificationMessageEntity message,
        NotificationTokenEntity token, CancellationToken cancellationToken)
    {
        SmtpClient client = null;
        try
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(EmailOptionBuilder.MailFromName, EmailOptionBuilder.MailFromLogin));
            mimeMessage.To.Add(new MailboxAddress(token.Token, token.Token));
            mimeMessage.Subject = message.Title;

            mimeMessage.Body = new TextPart("plain")
            {
                Text = message.Payload
            };

            client = new SmtpClient();
            await client.ConnectAsync(EmailOptionBuilder.SmtpHost, EmailOptionBuilder.SmtpPort,
                MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(EmailOptionBuilder.MailFromLogin, EmailOptionBuilder.MailFromPassword, cancellationToken);

            var response = await client.SendAsync(mimeMessage, cancellationToken);
            return SentStatus.Success(response);
        }
        catch (Exception e)
        {
            return SentStatus.Fail(e);
        }
        finally
        {
            if (client != null)
            {
                await client.DisconnectAsync(true, cancellationToken);
                client.Dispose();
            }
        }
    }
}