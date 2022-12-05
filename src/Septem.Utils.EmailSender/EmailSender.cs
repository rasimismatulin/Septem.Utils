using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using MailKit.Net.Smtp;

namespace Septem.Utils.EmailSender;

internal class EmailSender : IEmailSender
{
    internal static EmailSenderOptions Options { get; set; }
    private readonly ILogger<EmailSender> _logger;
    private static readonly object SyncRoot = new();

    private readonly MailboxAddress _senderAddress;
    private readonly ICollection<MailboxAddress> _receiverAddresses;
    private SmtpClient _smtpClient;

    public EmailSender(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EmailSender>();
        _senderAddress = MailboxAddress.Parse(Options.SmtpSender);
        _receiverAddresses = Options.Receiver.Split(',').Select(x => MailboxAddress.Parse(x.Trim())).ToArray();
    }

    private async ValueTask<SmtpClient> GetSmtpClient(CancellationToken cancellationToken)
    {
        lock (SyncRoot)
        {
            if (_smtpClient == null)
                _smtpClient = new SmtpClient();
            else
                return _smtpClient;
        }
        await _smtpClient.ConnectAsync(Options.SmtpHost, Options.SmtpPort, false, cancellationToken);
        await _smtpClient.AuthenticateAsync(Options.SmtpUser, Options.SmtpPassword, cancellationToken);
        return _smtpClient;
    }

    public async Task SendAsync(string body, EmailLevel level, string subject = null, CancellationToken cancellationToken = default)
    {
        if (level < Options.MinLevel || string.IsNullOrWhiteSpace(body))
            return;

        try
        {
            var bodyBuilder = new BodyBuilder
            {
                TextBody = body
            };

            var finalSubject = $"[{(string.IsNullOrWhiteSpace(subject) ? Options.Subject : subject).ToUpper()} - {level.ToString().ToUpper()}]";

            var message = new MimeMessage
            {
                Subject = finalSubject,
                Body = bodyBuilder.ToMessageBody(),
            };

            message.From.Add(_senderAddress);
            message.To.AddRange(_receiverAddresses);

            var client = await GetSmtpClient(cancellationToken);
            await client.SendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during send email");
        }
    }

    #region Helper methods

    public async Task SendDebugAsync(string body, string subject = null, CancellationToken cancellationToken = default) =>
        await SendAsync(body, EmailLevel.Debug, subject, cancellationToken);

    public async Task SendInformationAsync(string body, string subject = null, CancellationToken cancellationToken = default) =>
        await SendAsync(body, EmailLevel.Information, subject, cancellationToken);

    public async Task SendWarningAsync(string body, string subject = null, CancellationToken cancellationToken = default) =>
        await SendAsync(body, EmailLevel.Warning, subject, cancellationToken);

    public async Task SendErrorAsync(string body, string subject = null, CancellationToken cancellationToken = default) =>
        await SendAsync(body, EmailLevel.Error, subject, cancellationToken);

    public async Task SendExceptionAsync(Exception exception, string body = null, string subject = null, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(body))
            sb.AppendLine($"Body: [{body}]\n");

        var stackTrace = exception.StackTrace;
        var source = exception.Source;
        sb.AppendLine("Exception messages: ");

        var cnt = 0;
        while (exception != null)
        {
            sb.AppendLine($"InnerLevel: [{++cnt}]; Message: [{exception.Message}]");
            exception = exception.InnerException;
        }

        sb.AppendLine($"\n Stack trace: [{stackTrace}]");
        sb.AppendLine($"Source: [{source}]");

        await SendAsync(body, EmailLevel.Error, subject, cancellationToken);
    }

    #endregion

    public void Dispose()
    {
        if (_smtpClient is not null)
        {
            _smtpClient.Disconnect(true);
            _smtpClient?.Dispose();
        }
    }
}