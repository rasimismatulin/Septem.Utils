using Septem.Notifications.Abstractions.Enums;
using System;

namespace Septem.Notifications.Abstractions.Models;

public class NotificationReceiver
{
    public Guid Id { get; set; }

    public Guid? TargetUid { get; set; }

    public string Token { get; set; }

    public ReceiverType ReceiverType { get; set; }

    public string Parameters { get; set; }
}