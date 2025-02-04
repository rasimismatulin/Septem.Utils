using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Septem.Notifications.Abstractions;

namespace Septem.Notifications.Core.Entities;

internal class NotificationMessageEntity
{
    [Key]
    [Column("id")]
    public Guid Uid { get; set; }

    public DateTimeOffset CreatedDateUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ModifiedDateUtc { get; set; }

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