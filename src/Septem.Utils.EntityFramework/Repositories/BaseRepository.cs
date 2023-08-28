using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Internal;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Septem.Utils.Domain;
using Septem.Utils.EntityFramework.Entities;

namespace Septem.Utils.EntityFramework.Repositories;

public class BaseRepository<TEntity, TContext>
    where TEntity : BaseEntity, new()
    where TContext : DbContext
{
    private readonly ICollection<Expression<Func<TEntity, bool>>> _deletionExpressions;

    protected readonly TContext DbContext;
    protected readonly IMapper Mapper;
    protected readonly ICollection<Action> AfterSaveActions = new List<Action>();

    public BaseRepository(TContext dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        Mapper = mapper;
        _deletionExpressions = new List<Expression<Func<TEntity, bool>>>();
    }

    protected void AddExistPredicate(Expression<Func<TEntity, bool>> predicate) =>
        _deletionExpressions.Add(predicate);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await DbContext.SaveChangesAsync(cancellationToken);
        foreach (var afterSaveAction in AfterSaveActions)
            afterSaveAction.Invoke();
    }


    private IDbContextTransaction _transaction;

    public async Task BeginTransaction(CancellationToken cancellationToken)
    {
        _transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransaction(CancellationToken cancellationToken)
    {
        if (_transaction != null)
            await _transaction.CommitAsync(cancellationToken);
    }


    public async Task WithTransaction(Func<Task> action, CancellationToken cancellationToken)
    {
        await BeginTransaction(cancellationToken);
        await action();
        await CommitTransaction(cancellationToken);
    }

    #region Remove

    public virtual async Task RemoveAsync(Guid uid, CancellationToken cancellationToken)
    {
        var entity = new TEntity { Uid = uid };
        DbContext.Set<TEntity>().Attach(entity);

        if (entity is BasePersistenceEntity persistenceEntity)
        {
            persistenceEntity.IsDeleted = true;
            persistenceEntity.ModifiedUtc = DateTime.UtcNow;
        }
        else
        {
            var dbEntry = DbContext.Entry(entity);

            foreach (var dbEntryCollection in dbEntry.Collections)
            {
                var accessor = dbEntryCollection.Metadata.GetCollectionAccessor();
                await dbEntryCollection.LoadAsync(cancellationToken);
                foreach (var child in dbEntryCollection.CurrentValue)
                {
                    DbContext.Remove(child);
                }
                //accessor.Remove(entity, child);
            }
            DbContext.Set<TEntity>().Remove(entity);
        }

    }

    public async Task RemoveAsync<TDomain>(TDomain domain, CancellationToken cancellationToken)
        where TDomain : BaseDomain =>
        await RemoveAsync(domain.Uid, cancellationToken);

    public async Task RemoveRangeAsync(IEnumerable<Guid> uidSource, CancellationToken cancellationToken)
    {
        foreach (var uid in uidSource)
            await RemoveAsync(uid, cancellationToken);
    }

    public async Task RemoveRangeAsync<TDomain>(IEnumerable<TDomain> domainSource, CancellationToken cancellationToken)
        where TDomain : BaseDomain
    {
        foreach (var domain in domainSource)
            await RemoveAsync(domain.Uid, cancellationToken);
    }

    #endregion

    #region Exists

    protected virtual IQueryable<TEntity> GetExistingSet() =>
        _deletionExpressions.Aggregate(DbContext.Set<TEntity>().AsNoTracking(), (current, expr) => current.Where(expr));

    public async Task<bool> ExistsAsync(Guid uid, CancellationToken cancellationToken) =>
        await GetExistingSet().AnyAsync(x => x.Uid == uid, cancellationToken);

    public async Task<bool> ExistsAsync<TDomain>(Expression<Func<TDomain, bool>> predicate, CancellationToken cancellationToken) =>
        await GetExistingSet().ProjectTo<TDomain>(Mapper.ConfigurationProvider).AnyAsync(predicate, cancellationToken);

    #endregion

    #region Get

    public IQueryable<TDomain> CollectionQuery<TDomain>()
        => GetExistingSet().ProjectTo<TDomain>(Mapper.ConfigurationProvider);

    public async Task<TDomain> GetAsync<TDomain>(Guid uid, CancellationToken cancellationToken)
        where TDomain : BaseDomain =>
        await GetExistingSet()
            .Where(x => x.Uid == uid)
            .ProjectTo<TDomain>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ICollection<TDomain>> GetAllAsync<TDomain>(CancellationToken cancellationToken)
        where TDomain : BaseDomain 
        => await CollectionQuery<TDomain>().ToArrayAsync(cancellationToken);

    #endregion

    #region Add

    public virtual async Task AddAsync<TDomain>(TDomain domain, CancellationToken cancellationToken)
        where TDomain : BaseDomain
    {
        var entity = Mapper.Map<TEntity>(domain);
        await DbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        AfterSaveActions.Add(() => domain.Uid = entity.Uid);
    }

    public async Task AddRangeAsync<TDomain>(IEnumerable<TDomain> domainSource, CancellationToken cancellationToken)
        where TDomain : BaseDomain
    {
        await DbContext.Set<TEntity>().AddRangeAsync(domainSource.Select(Mapper.Map<TEntity>), cancellationToken);
    }


    //public async Task UpdateAsync<TDomain>(TDomain domain, CancellationToken cancellationToken)
    //    where TDomain : BaseDomain
    //{
    //    var query = _deletionExpressions.Aggregate(DbContext.Set<TEntity>().AsQueryable(), (current, expr) => current.Where(expr));
    //    var entity = await query.Where(x => x.Uid == domain.Uid).FirstOrDefaultAsync(cancellationToken);

    //    if (entity == default)
    //        return;

    //    Mapper.Map(domain, entity);
    //}

    public virtual async Task UpdateAsync<TDomain>(TDomain domain, CancellationToken cancellationToken)
        where TDomain : BaseDomain
    {
        await UpdateInternalAsync(domain);
    }

    private static IEnumerable<KeyValuePair<string, object>> GetCollectionProperties(Type type, object obj)
    {
        foreach (var propertyInfo in type.GetProperties())
        {
            if (propertyInfo.PropertyType.Name == "ICollection`1")
            {
                var collectionValue = propertyInfo.GetValue(obj);
                if (collectionValue != null)
                    yield return new KeyValuePair<string, object>(propertyInfo.Name, collectionValue);
            }
        }
    }

    private async Task UpdateInternalAsync(object domainObject)
    {
        var domain = (BaseDomain)domainObject;

        var domainType = domain.GetType();
        var entityTypeBase = typeof(BaseEntity);
        var type = Mapper.ConfigurationProvider.Internal().GetAllTypeMaps()
            .First(x => x.SourceType == domainType && x.DestinationType.IsAssignableTo(entityTypeBase))
            .DestinationType;

        var dbEntity = await DbContext.FindAsync(type, domain.Uid);
        if (dbEntity != null)
        {
            var navigationProperties = GetCollectionProperties(domainObject.GetType(), domainObject).ToList();
            Mapper.Map(domain, dbEntity);

            var dbEntry = DbContext.Entry(dbEntity);
            foreach (var (propertyName, collection) in navigationProperties)
            {
                var dbItemsEntry = dbEntry.Collection(propertyName);
                var accessor = dbItemsEntry.Metadata.GetCollectionAccessor();

                if (accessor == null)
                    continue;

                await dbItemsEntry.LoadAsync();
                var dbItemsMap = (((IEnumerable<BaseEntity>)dbItemsEntry.CurrentValue) ?? Array.Empty<BaseEntity>()).ToDictionary(e => e.Uid);

                var items = (IEnumerable<BaseDomain>)collection;

                foreach (var item in items)
                {
                    if (!dbItemsMap.ContainsKey(item.Uid))
                    {
                        var itemType = accessor.CollectionType.GenericTypeArguments[0];
                        var result = Mapper.Map(item, item.GetType(), itemType);
                        accessor.Add(dbEntity, result, false);
                    }
                    else
                    {
                        await UpdateInternalAsync(item);
                        dbItemsMap.Remove(item.Uid);
                    }
                }

                foreach (var oldItem in dbItemsMap.Values)
                {
                    if (oldItem is BasePersistenceEntity basePersistenceEntity)
                    {
                        basePersistenceEntity.IsDeleted = true;
                        basePersistenceEntity.ModifiedUtc = DateTime.UtcNow;
                    }
                    else
                    {
                        DbContext.Remove(oldItem);
                    }
                }
            }
        }
    }
    #endregion

}