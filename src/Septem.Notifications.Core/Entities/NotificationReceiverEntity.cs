using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Septem.Notifications.Abstractions;

namespace Septem.Notifications.Core.Entities;

internal class NotificationReceiverEntity
{
    [Key]
    public Guid Uid { get; set; }

    public Guid? TargetUid { get; set; }

    [StringLength(250)]
    public string Token { get; set; }

    public ReceiverType ReceiverType { get; set; }

    [StringLength(500)]
    public string Parameters { get; set; }

    public Guid NotificationUid { get; set; }

    [ForeignKey(nameof(NotificationUid))]
    public NotificationEntity Notification { get; set; }
}