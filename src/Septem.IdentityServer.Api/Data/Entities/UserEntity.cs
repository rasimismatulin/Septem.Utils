using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Septem.IdentityServer.Api.Data.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        public Guid Uid { get; set; }

        public bool IsDeleted { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string SecretHash { get; set; }

        public string SecretSalt { get; set; }

        public byte Status { get; set; }

        public byte UserRole { get; set; }

        public string Language { get; set; }
    }
}
