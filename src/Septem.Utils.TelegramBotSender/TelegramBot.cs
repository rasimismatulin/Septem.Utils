using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Septem.Utils.TelegramBotSender;

internal class TelegramBot : ITelegramBotSender
{
    private readonly string _botToken;
    private readonly string _botUsername;
    internal static TelegramBotOptions Options { get; set; }
    private readonly ILogger<TelegramBot> _logger;
    private static readonly object SyncRoot = new();

    private TelegramBotClient _telegramBotClient;

    public TelegramBot(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TelegramBot>();
        _botToken = Options.BotToken;
        _botUsername = Options.BotUsername;

    }

    private TelegramBotClient GetTelegramBotClient()
    {
        lock (SyncRoot)
        {
            if (_telegramBotClient == null)
                _telegramBotClient = new TelegramBotClient(_botToken);
            else
                return _telegramBotClient;
        }
        return _telegramBotClient;
    }

    public async Task SendAsync(string body, MessageLevel level, string subject = null, CancellationToken cancellationToken = default)
    {
        if (level < Options.MinLevel || string.IsNullOrWhiteSpace(body))
            return;

        try
        {
            var client = GetTelegramBotClient();
            var message = $"{subject}\n{body}";
            await client.SendTextMessageAsync(new ChatId(_botUsername), message, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during send email");
        }
    }

    #region Helper methods

    public async Task SendDebugAsync(string body, string subject = null, CancellationToken cancellationToken = default) =>
        await SendAsync(body, MessageLevel.Debug, subject, cancellationToken);

    public async Task SendInformationAsync(string body, string subject = null, CancellationToken cancellationToken = default) =>
        await SendAsync(body, MessageLevel.Information, subject, cancellationToken);

    public async Task SendWarningAsync(string body, string subject = null, CancellationToken cancellationToken = default) =>
        await SendAsync(body, MessageLevel.Warning, subject, cancellationToken);

    public async Task SendErrorAsync(string body, string subject = null, CancellationToken cancellationToken = default) =>
        await SendAsync(body, MessageLevel.Error, subject, cancellationToken);

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

        await SendAsync(body, MessageLevel.Error, subject, cancellationToken);
    }

    #endregion

    public void Dispose()
    {
    }
}