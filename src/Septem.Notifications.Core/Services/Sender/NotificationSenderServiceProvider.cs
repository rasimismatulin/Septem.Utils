using System;
using Microsoft.Extensions.DependencyInjection;
using Septem.Notifications.Abstractions;

namespace Septem.Notifications.Core.Services.Sender;

internal class NotificationSenderServiceProvider
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationSenderServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public INotificationSenderService GetByTokenType(NotificationTokenType type)
    {
        return type switch
        {
            NotificationTokenType.Sms => _serviceProvider.GetService<SmsNotificationSenderService>(),
            NotificationTokenType.Email => _serviceProvider.GetService<EmailNotificationSenderService>(),
            NotificationTokenType.Fcm => _serviceProvider.GetService<FcmNotificationSenderService>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}