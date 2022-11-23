using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Septem.IdentityServer.Api.Data.Entities
{
    public class ApplicationDbContextFactory
    {
        public ApplicationDbContextFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ApplicationDbContext CreateApplicationDbContext(string connectionName)
        {
            return new ApplicationDbContext(new DbContextOptionsBuilder().UseNpgsql(Configuration.GetConnectionString(connectionName)).Options);
        }

        public IConfiguration Configuration { get; }
    }
}
