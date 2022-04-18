using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Septem.Notifications.Core.Entities;

internal class NotificationEntity
{
    [Key]
    public Guid Uid { get; set; }

    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime TimeToSendUtc { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

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