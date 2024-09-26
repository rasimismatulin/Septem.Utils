using System;
using Serilog;
using Serilog.Configuration;

namespace Septem.Utils.TelegramBotSender;

public static class TelegramSinkExtensions
{
    public static LoggerConfiguration TelegramSink(
        this LoggerSinkConfiguration loggerConfiguration,
        string botToken,
        string url,
        ChatLevelConfiguration[] chatLevels,
        bool enableInDebugLogs,
        string prefix,
        IFormatProvider formatProvider = null)
    {
        var telegramConfig = new TelegramSettings
        {
            BotToken = botToken,
            Url = url,
            ChatLevels = chatLevels,
            EnableInDebugLogs = enableInDebugLogs,
            Prefix = prefix
        };

        TelegramSender.Instance = new TelegramSender(telegramConfig);
        return loggerConfiguration.Sink(new TelegramSink(formatProvider));
    }
}