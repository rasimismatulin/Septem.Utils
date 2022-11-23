using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Septem.Utils.EntityFramework.Entities;

public abstract class BasePersistenceEntity : BaseEntity
{
    [Required]
    [Column("CreatedUtc")]
    public DateTime CreatedUtc { get; set; }

    [Column("ModifiedUtc")]
    public DateTime? ModifiedUtc { get; set; }

    [Required]
    [Column("IsDeleted")]
    public bool IsDeleted { get; set; }
}