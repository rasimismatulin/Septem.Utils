using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Septem.Utils.EntityFramework.Dynamic
{
    [Table("test")]
    public class tnp
    {
        [Key]
        [Column("test")]
        public string Name { get; set; }
    }
}
