using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Septem.Notifications.Abstractions;

namespace Septem.Notifications.Core.Entities;

internal class NotificationMessageEntity
{
    [Key]
    public Guid Uid { get; set; }

    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDateUtc { get; set; }

    public bool IsDeleted { get; set; } = false;

    public NotificationMessageStatus Status { get; set; } = NotificationMessageStatus.Pending;


    [StringLength(250)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Payload { get; set; }

    [StringLength(250)]
    public string Token { get; set; }

    public NotificationTokenType? TokenType { get; set; }


    public Guid? NotificationTokenUid { get; set; }

    [ForeignKey(nameof(NotificationTokenUid))]
    public NotificationTokenEntity NotificationToken { get; set; }

    public Guid NotificationUid { get; set; }

    [ForeignKey(nameof(NotificationUid))]
    public NotificationEntity Notification { get; set; }


    [StringLength(500)]
    public string ServiceMessage { get; set; }

    [StringLength(500)]
    public string ExceptionMessage { get; set; }

    public bool IsView { get; set; }
}