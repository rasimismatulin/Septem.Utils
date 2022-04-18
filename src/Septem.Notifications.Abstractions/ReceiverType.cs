namespace Septem.Notifications.Abstractions;

public enum ReceiverType : byte
{
    Sms,
    Email,
    Fcm,
    FcmOrSms
}