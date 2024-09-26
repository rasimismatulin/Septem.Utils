using Serilog.Events;

namespace Septem.Utils.TelegramBotSender;

public class TelegramSettings
{
    public string BotToken { get; set; }

    public string Url { get; set; }

    public ChatLevelConfiguration[] ChatLevels { get; set; }

    public bool EnableInDebugLogs { get; set; }

    public string Prefix { get; set; }
}

public class ChatLevelConfiguration
{
    public LogEventLevel[] Levels { get; set; }

    public long ChatId { get; set; }
}