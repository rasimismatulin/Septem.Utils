using System;

namespace Septem.Notifications.Abstractions;

public class NotificationToken
{
    public Guid Uid { get; set; }

    public Guid TargetUid { get; set; }

    public string Token { get; set; }

    public string DeviceId { get; set; }

    public NotificationTokenType Type { get; set; }

    public string Language { get; set; }

    public NotificationToken()
    {

    }

    public NotificationToken(Guid targetUid, string token, NotificationTokenType type, string deviceId = default, string language = default)
    {
        TargetUid = targetUid;
        Token = token;
        Type = type;
        DeviceId = deviceId;
        Language = language;
    }

    public static NotificationToken Sms(Guid targetUid, string phoneNumber, string language = default) => new(targetUid, phoneNumber, NotificationTokenType.Sms, default, language);

    public static NotificationToken Email(Guid targetUid, string email, string language = default) => new(targetUid, email, NotificationTokenType.Email, default, language);

    public static NotificationToken Fcm(Guid targetUid, string token, string language = default) => new(targetUid, token, NotificationTokenType.Fcm, default, language);

    public static NotificationToken FcmWithDevice(Guid targetUid, string token, string deviceUid, string language = default) => new(targetUid, token, NotificationTokenType.Fcm, deviceUid, language);
}