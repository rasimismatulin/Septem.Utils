using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Infrastructure;

internal static class Converter
{
    public static NotificationEntity GetNotificationEntity(Notification notification)
    {
        return new NotificationEntity
        {
            Uid = default,
            CreatedDateUtc = DateTimeOffset.UtcNow,
            TimeToSendUtc = notification.TimeToSendUtc,
            IsDeleted = false,
            ModifiedDateUtc = default,
            Status = NotificationStatus.Pending,

            Title = notification.Title,
            Payload = notification.Payload,
            Data = notification.Data,
            DefaultLanguage = notification.DefaultLanguage,
            Type = notification.Type,
            GroupKey = notification.GroupKey,
            CancellationKey = notification.CancellationKey,
            FcmConfiguration = notification.FcmConfiguration == null ? null : JsonSerializer.Serialize(notification.FcmConfiguration),
            Receivers = new List<NotificationReceiverEntity>(),
            Messages = new List<NotificationMessageEntity>()
        };
    }
    public static Notification GetNotification(NotificationEntity entity)
    {
        return new Notification
        {
            Uid = entity.Uid,
            TimeToSendUtc = entity.TimeToSendUtc,
            Title = entity.Title,
            Payload = entity.Payload,
            Data = entity.Data,
            DefaultLanguage = entity.DefaultLanguage,
            GroupKey = entity.GroupKey,
            CancellationKey = entity.CancellationKey,
            Type = entity.Type,
            FcmConfiguration = entity.FcmConfiguration == null ? null : JsonSerializer.Deserialize<FcmConfiguration>(entity.FcmConfiguration)
        };
    }

    public static NotificationReceiverEntity GetReceiverEntity(Receiver receiver)
    {
        return new NotificationReceiverEntity
        {
            Token = receiver.Token,
            TargetUid = receiver.TargetUid,
            ReceiverType = receiver.ReceiverType,
            Parameters = receiver.Parameters.Any() ? JsonSerializer.Serialize(receiver.Parameters) : default,
        };
    }

    public static NotificationTokenEntity GetNotificationTokenEntity(NotificationToken notificationToken)
    {
        if (notificationToken == null)
            return null;

        return new NotificationTokenEntity
        {
            Uid = notificationToken.Uid,
            Token = notificationToken.Token,
            TargetUid = notificationToken.TargetUid,
            Type = notificationToken.Type,
            Language = notificationToken.Language,
            DeviceId = notificationToken.DeviceId
        };
    }

    public static NotificationToken GetNotificationToken(NotificationTokenEntity entity)
    {
        if (entity == null)
            return null;

        return new NotificationToken
        {
            Uid = entity.Uid,
            Token = entity.Token,
            TargetUid = entity.TargetUid,
            Type = entity.Type,
            DeviceId = entity.DeviceId
        };
    }

    public static NotificationTokenType? ToTokenType(ReceiverType receiverType)
    {
        switch (receiverType)
        {
            case ReceiverType.Sms:
                return NotificationTokenType.Sms;
            case ReceiverType.Email:
                return NotificationTokenType.Email;
            case ReceiverType.Fcm:
                return NotificationTokenType.Fcm;
            case ReceiverType.FcmOrSms:
                return NotificationTokenType.Fcm;
            default:
                throw new ArgumentOutOfRangeException(nameof(receiverType), receiverType, null);
        }
    }
}