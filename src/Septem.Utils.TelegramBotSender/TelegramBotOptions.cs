
namespace Septem.Utils.TelegramBotSender;

public class TelegramBotOptions
{
    public TelegramBotConfiguration ReadFrom { get; set; }

    public TelegramBotOptions()
    {
        ReadFrom = new TelegramBotConfiguration(this);
    }

    public string BotToken { get; set; }

    public string BotUsername { get; set; }

    public MessageLevel MinLevel { get; set; }

    public ContainerType ContainerType { get; set; }

    public static TelegramBotOptions Default => new()
    {
        MinLevel = MessageLevel.Warning,
        ContainerType = ContainerType.Transient
    };
}