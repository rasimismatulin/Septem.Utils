using System;
using Microsoft.Extensions.DependencyInjection;

namespace Septem.Utils.EmailSender;

public static class ServiceCollectionExtensions
{
    public static void AddEmailSender(this IServiceCollection services, Action<EmailSenderOptions> configureOptions = null)
    {
        var options = EmailSenderOptions.Default;
        configureOptions?.Invoke(options);
        EmailSender.Options = options;
        switch (options.ContainerType)
        {
            case ContainerType.Transient:
                services.AddTransient<IEmailSender, EmailSender>();
                break;
            case ContainerType.Scoped:
                services.AddScoped<IEmailSender, EmailSender>();
                break;
            case ContainerType.Singleton:
                services.AddSingleton<IEmailSender, EmailSender>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}