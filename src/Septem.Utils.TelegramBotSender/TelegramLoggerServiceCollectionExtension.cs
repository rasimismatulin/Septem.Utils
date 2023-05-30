using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Septem.Utils.TelegramBotSender;

public static class TelegramLoggerServiceCollectionExtension
{
    public static IServiceCollection AddTelegramLogger(this IServiceCollection services, IConfiguration configuration)
    {
        var telegramConfig = configuration.GetSection("TelegramSettings").Get<TelegramSettings>();
        TelegramSender.Instance = new TelegramSender(telegramConfig);
        return services;
    }
}