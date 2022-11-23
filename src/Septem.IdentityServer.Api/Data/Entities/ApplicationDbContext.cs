using Microsoft.EntityFrameworkCore;

namespace Septem.IdentityServer.Api.Data.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserEntity> UserEntities { get; set; }
    }
}
