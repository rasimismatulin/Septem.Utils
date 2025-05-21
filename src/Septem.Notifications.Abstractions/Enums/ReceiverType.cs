using System;

namespace Septem.Notifications.Abstractions.Enums;

[Flags]
public enum ReceiverType
{
    None = 0,
    Sms = 1 << 0,
    Email = 1 << 1,
    Fcm = 1 << 2
}