using Serilog.Events;

namespace Septem.Utils.TelegramBotSender;

public class TelegramLevelSettings
{
    public LogEventLevel[] Levels { get; set; }

    public long ChatId { get; set; }
}