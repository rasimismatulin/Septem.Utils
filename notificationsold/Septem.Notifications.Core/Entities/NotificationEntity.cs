using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Septem.Notifications.Core.Entities;

internal class NotificationEntity
{
    [Key]
    [Column("id")]
    public Guid Uid { get; set; }

    public DateTimeOffset CreatedDateUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset TimeToSendUtc { get; set; }

    public DateTimeOffset? ModifiedDateUtc { get; set; }

    public bool IsDeleted { get; set; } = false;

    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    public byte Type { get; set; }

    [StringLength(250)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Payload { get; set; }

    [StringLength(500)]
    public string Data { get; set; }

    [StringLength(10)]
    public string DefaultLanguage { get; set; }

    public Guid? GroupKey { get; set; }

    [StringLength(50)]
    public string CancellationKey { get; set; }

    [StringLength(500)]
    public string FcmConfiguration { get; set; }


    public virtual ICollection<NotificationReceiverEntity> Receivers { get; set; }

    public virtual ICollection<NotificationMessageEntity> Messages { get; set; }

}