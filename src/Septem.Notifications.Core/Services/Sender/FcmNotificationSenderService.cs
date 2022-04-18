using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Septem.Notifications.Core.Entities;
using CoreNotification = Septem.Notifications.Abstractions.Notification;

namespace Septem.Notifications.Core.Services.Sender;

internal class FcmNotificationSenderService : INotificationSenderService
{
    public async Task<SentStatus> SendAsync(CoreNotification notification, NotificationMessageEntity message, NotificationTokenEntity token, CancellationToken cancellationToken)
    {
        try
        {
            var messageNotification = new Notification
            {
                Title = message.Title,
                Body = message.Payload
            };

            var fcmMessage = new Message
            {
                Notification = messageNotification,
                Token = token.Token
            };

            if (notification.FcmConfiguration?.Data != default)
                fcmMessage.Data = notification.FcmConfiguration.Data;

            fcmMessage.Android = new AndroidConfig
            {
                Priority = Priority.High,
                Notification = new AndroidNotification
                {
                    ChannelId = "main_notification_channel_id",
                    Sound = "default",
                    Color = notification.FcmConfiguration?.Color
                }
            };

            fcmMessage.Apns = new ApnsConfig
            {
                Headers = new Dictionary<string, string>
                {
                    { "apns-priority", "10" }
                },
                Aps = new Aps
                {
                    ContentAvailable = true,
                    Badge = notification.FcmConfiguration?.BadgeCount,
                    Sound = "default"
                }
            };

            var instanceName = string.IsNullOrWhiteSpace(notification.FcmConfiguration?.InstanceName)
                ? "Default"
                : notification.FcmConfiguration.InstanceName;

            var msg = FirebaseMessaging.GetMessaging(FirebaseApp.GetInstance(instanceName));
            var response = await msg.SendAsync(fcmMessage, cancellationToken);

            return SentStatus.Success(response);
        }
        catch (Exception e)
        {
            return SentStatus.Fail(e);
        }
    }
}