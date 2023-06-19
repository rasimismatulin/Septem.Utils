using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog.Events;

namespace Septem.Utils.TelegramBotSender;

public class TelegramSender
{
    public static TelegramSender Instance { get; set; } = default;

    private readonly string _telegramUrl;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<LogEventLevel, long> _levelChats;
    private readonly bool _enableInDebugLogs;
    private readonly string _prefix;

    public TelegramSender(TelegramSettings settings)
    {
        _enableInDebugLogs = settings.EnableInDebugLogs;
        _prefix = settings.Prefix;
        _telegramUrl = string.Format(settings.Url, settings.BotToken);
        _httpClient = new HttpClient();
        _levelChats = new Dictionary<LogEventLevel, long>();
        foreach (var chatLevel in settings.ChatLevels)
        {
            foreach (var logEventLevel in chatLevel.Levels)
            {
                _levelChats[logEventLevel] = chatLevel.ChatId;
            }
        }
    }

    public void Send(LogEventLevel level, string message)
    {
        try
        {
            if (Debugger.IsAttached)
            {
                if (!_enableInDebugLogs)
                    return;

                message = $"[DEBUG] {message}";
            }

            if (!string.IsNullOrWhiteSpace(_prefix))
                message = $"{_prefix}{message}";

            var exists = _levelChats.TryGetValue(level, out var chatId);
            if (!exists)
                return;

            var content = new { chat_id = chatId, text = message, disable_web_page_preview = true };
            Task.Run(() => _httpClient.PostAsync(_telegramUrl,
                    new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json"))
                .ConfigureAwait(false));
        }
        catch (Exception)
        {
            // ignored
        }
    }
}