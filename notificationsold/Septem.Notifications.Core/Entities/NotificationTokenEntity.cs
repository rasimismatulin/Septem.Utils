using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Septem.Notifications.Abstractions;

namespace Septem.Notifications.Core.Entities;

internal class NotificationTokenEntity
{
    [Key]
    [Column("id")]
    public Guid Uid { get; set; }

    public DateTimeOffset CreatedDateUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ModifiedDateUtc { get; set; }

    public bool IsDeleted { get; set; } = false;

    public Guid TargetUid { get; set; }

    [StringLength(50)]
    public string DeviceId { get; set; }

    [StringLength(250)]
    public string Token { get; set; }

    [StringLength(10)]
    public string Language { get; set; }

    public NotificationTokenType Type { get; set; }
}