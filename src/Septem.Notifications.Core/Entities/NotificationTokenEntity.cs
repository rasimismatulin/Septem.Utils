using System;
using System.ComponentModel.DataAnnotations;
using Septem.Notifications.Abstractions;

namespace Septem.Notifications.Core.Entities;

internal class NotificationTokenEntity
{
    [Key]
    public Guid Uid { get; set; }

    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDateUtc { get; set; }

    public bool IsDeleted { get; set; } = false;

    public Guid TargetUid { get; set; }

    [StringLength(50)]
    public string DeviceId { get; set; }

    [StringLength(250)]
    public string Token { get; set; }

    public string Language { get; set; }

    public NotificationTokenType Type { get; set; }
}