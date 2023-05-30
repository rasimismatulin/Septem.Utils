using System;
using System.Text;
using Serilog.Core;
using Serilog.Events;

namespace Septem.Utils.TelegramBotSender;

public class TelegramSink : ILogEventSink
{
    private readonly IFormatProvider _formatProvider;

    public TelegramSink(IFormatProvider formatProvider)
    {
        _formatProvider = formatProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        if (TelegramSender.Instance == default)
            return;

        var message = logEvent.RenderMessage(_formatProvider);
        if (logEvent.Exception != null)
            message += GetExceptionMessages(logEvent.Exception);

        TelegramSender.Instance.Send(logEvent.Level, message);
    }

    private static string GetExceptionMessages(Exception exception)
    {
        var sb = new StringBuilder();
        sb.Append(Environment.NewLine);
        sb.Append("Exception: ");
        sb.Append(exception.GetType().Name);
        sb.Append(Environment.NewLine);

        sb.Append("Message: ");
        while (exception != null)
        {
            sb.Append(exception.Message);
            exception = exception.InnerException;
        }
        return sb.ToString();
    }
}