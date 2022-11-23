using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Septem.Utils.EntityFramework.Entities;
using Septem.Utils.Helpers.ActionInvoke.Collection;

namespace Septem.Utils.EntityFramework.Repositories;

public class BaseSearchPersistenceRepository<TEntity, TContext, TSearch> : BaseSearchRepository<TEntity, TContext, TSearch>
    where TSearch : CollectionQuery
    where TEntity : BasePersistenceEntity, new()
    where TContext : DbContext
{
    public BaseSearchPersistenceRepository(TContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
        AddExistPredicate(e => !e.IsDeleted);
    }

    public override Task RemoveAsync(Guid uid, CancellationToken cancellationToken)
    {
        var entity = new TEntity { Uid = uid, IsDeleted = true };
        DbContext.Set<TEntity>().Attach(entity);
        DbContext.Entry(entity).Property(x => x.IsDeleted).IsModified = true;
        return Task.CompletedTask;
    }
}