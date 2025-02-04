using System;
using Microsoft.Extensions.DependencyInjection;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Services.Sender.Sms;

namespace Septem.Notifications.Core.Config;

public delegate string RestSmsUrlBuilder(IServiceProvider serviceProvider, Notification notification, string token, string payload);
public delegate bool RestSmsResponseValidator(IServiceProvider serviceProvider, Notification notification, string response);

public class SmsOptionBuilder
{
    private readonly IServiceCollection _services;
    internal static RestSmsUrlBuilder UrlBuilder;
    internal static RestSmsResponseValidator Validator;

    public SmsOptionBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public void Default(RestSmsUrlBuilder urlBuilder, RestSmsResponseValidator validator)
    {
        _services.AddTransient<SmsNotificationSenderService>();
        UrlBuilder = urlBuilder;
        Validator = validator;
    }
}