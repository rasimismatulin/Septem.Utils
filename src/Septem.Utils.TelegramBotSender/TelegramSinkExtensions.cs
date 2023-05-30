using System;
using Serilog;
using Serilog.Configuration;

namespace Septem.Utils.TelegramBotSender;

public static class TelegramSinkExtensions
{
    public static LoggerConfiguration TelegramSink(
        this LoggerSinkConfiguration loggerConfiguration,
        IFormatProvider formatProvider = null)
    {
        return loggerConfiguration.Sink(new TelegramSink(formatProvider));
    }
}