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
        services.AddTransient<IEmailSender, EmailSender>();
    }
}