using System;
using Microsoft.Extensions.Configuration;

namespace Septem.Utils.TelegramBotSender;

public class TelegramBotConfiguration
{
    private readonly TelegramBotOptions _options;

    public TelegramBotConfiguration(TelegramBotOptions options)
    {
        _options = options;
    }

    public void Configuration(IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection("TelegramBotSender");
        _options.MinLevel = (MessageLevel)Enum.Parse(typeof(MessageLevel), section["MinLevel"], true);
        _options.BotToken = section["BotToken"];
        _options.BotUsername = section["BotUsername"];
    }
}