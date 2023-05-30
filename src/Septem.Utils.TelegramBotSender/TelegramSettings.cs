namespace Septem.Utils.TelegramBotSender;

public class TelegramSettings
{
    public string BotToken { get; set; }

    public string Url { get; set; }

    public TelegramLevelSettings[] ChatLevels { get; set; }

    public bool EnableInDebugLogs { get; set; }
}