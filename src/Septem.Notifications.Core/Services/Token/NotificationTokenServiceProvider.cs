using System;
using Microsoft.Extensions.DependencyInjection;
using Septem.Notifications.Abstractions;

namespace Septem.Notifications.Core.Services.Token;

internal class NotificationTokenServiceProvider
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationTokenServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public INotificationTokenFindService GetByReceiverType(ReceiverType type)
    {
        return type switch
        {
            ReceiverType.Sms => _serviceProvider.GetService<SmsNotificationTokenService>(),
            ReceiverType.Email => _serviceProvider.GetService<EmailNotificationTokenService>(),
            ReceiverType.Fcm => _serviceProvider.GetService<FcmNotificationTokenService>(),
            ReceiverType.FcmOrSms => _serviceProvider.GetService<FcmOrSmsNotificationTokenService>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}