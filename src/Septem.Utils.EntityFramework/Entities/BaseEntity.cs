using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Septem.Utils.EntityFramework.Entities;

public abstract class BaseEntity
{
    [Key]
    [Required]
    [Column("Uid")]
    public Guid Uid { get; set; }
}