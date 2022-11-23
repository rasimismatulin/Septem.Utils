using Microsoft.EntityFrameworkCore;
using Septem.Utils.EntityFramework.Entities;

namespace Septem.Utils.EntityFramework.Repositories
{
    public class BaseVerificationRepository<TEntity, TContext>
        where TEntity : BaseEntity, new ()
        where TContext : DbContext
    {
    }
}
