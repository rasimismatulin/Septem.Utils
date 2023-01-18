using System;
using Microsoft.Extensions.DependencyInjection;

namespace Septem.Utils.TelegramBotSender;

public static class ServiceCollectionExtensions
{
    public static void AddEmailSender(this IServiceCollection services, Action<TelegramBotSenderOptions> configureOptions = null)
    {
        var options = TelegramBotSenderOptions.Default;
        configureOptions?.Invoke(options);
        TelegramBotSender.Options = options;
        switch (options.ContainerType)
        {
            case ContainerType.Transient:
                services.AddTransient<ITelegramSender, TelegramSender>();
                break;
            case ContainerType.Scoped:
                services.AddScoped<ITelegramSender, TelegramSender>();
                break;
            case ContainerType.Singleton:
                services.AddSingleton<ITelegramSender, TelegramSender>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}