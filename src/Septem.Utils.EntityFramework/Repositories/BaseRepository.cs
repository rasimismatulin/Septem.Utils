using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Internal;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Septem.Utils.Domain;
using Septem.Utils.EntityFramework.Entities;

namespace Septem.Utils.EntityFramework.Repositories;

public class BaseRepository<TEntity, TContext>
    where TEntity : BaseEntity, new()
    where TContext : DbContext
{
    public void SyncCollections<TKey, TSource, TElement>(
        ICollection<TSource> sourceCollection, Func<TSource, TKey> sourceKeySelector,
        ICollection<TElement> destinationCollection, Func<TElement, TKey> destinationKeySelector,
        Func<TKey, TElement> newElementFactory)
    {
        var valuesCache = destinationCollection.Select(destinationKeySelector).ToHashSet();

        foreach (var item in sourceCollection)
        {
            var key = sourceKeySelector(item);
            if (!valuesCache.Contains(key))
                destinationCollection.Add(newElementFactory(key));
            else
                valuesCache.Remove(key);
        }

        foreach (var oldItem in valuesCache)
        {
            var toRemove = destinationCollection.FirstOrDefault(x => destinationKeySelector(x).Equals(oldItem));
            if (toRemove is not null)
                destinationCollection.Remove(toRemove);
        }
    }
    public void SyncCollections<TSource, TElement>(ICollection<TSource> sourceCollection,
        ICollection<TElement> destinationCollection, Func<Guid, TElement> newElementFactory)
        where TElement : BaseEntity, new()
        where TSource : BaseDomain, new()
    {
        var valuesCache = destinationCollection.Select(x => x.Uid).ToHashSet();

        foreach (var item in sourceCollection)
        {
            if (!valuesCache.Contains(item.Uid))
                destinationCollection.Add(newElementFactory(item.Uid));
            else
                valuesCache.Remove(item.Uid);
        }

        foreach (var oldItem in valuesCache)
        {
            var toRemove = destinationCollection.FirstOrDefault(x => x.Uid == oldItem);
            destinationCollection.Remove(toRemove);
        }
    }


    private static readonly Expression<Func<TEntity, bool>> PersistenceDeletedExpression;
    private readonly ICollection<Expression<Func<TEntity, bool>>> _deletionExpressions;
    protected readonly ICollection<Action> AfterSaveActions = new List<Action>();

    protected readonly TContext DbContext;
    protected readonly IMapper Mapper;

    static BaseRepository()
    {
        if (!typeof(BasePersistenceEntity).IsAssignableFrom(typeof(TEntity)))
            return;

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
        var notDeletedExpressionTree = Expression.Not(isDeletedProperty);

        PersistenceDeletedExpression = Expression.Lambda<Func<TEntity, bool>>(notDeletedExpressionTree, parameter);
    }

    public BaseRepository(TContext dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        Mapper = mapper;
        _deletionExpressions = new List<Expression<Func<TEntity, bool>>>();

        if (PersistenceDeletedExpression != default)
            AddExistPredicate(PersistenceDeletedExpression);
    }

    protected void AddExistPredicate(Expression<Func<TEntity, bool>> predicate) =>
        _deletionExpressions.Add(predicate);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await DbContext.SaveChangesAsync(cancellationToken);
        foreach (var afterSaveAction in AfterSaveActions)
            afterSaveAction.Invoke();
        AfterSaveActions.Clear();
    }

    private IDbContextTransaction _transaction;

    public async Task BeginTransaction(CancellationToken cancellationToken)
    {
        if (DbContext.Database.CurrentTransaction is null)
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
                //var accessor = dbEntryCollection.Metadata.GetCollectionAccessor();
                await dbEntryCollection.LoadAsync(cancellationToken);

                if (dbEntryCollection.CurrentValue != null)
                {
                    foreach (var child in dbEntryCollection.CurrentValue)
                    {
                        DbContext.Remove(child);
                    }
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

    protected virtual IQueryable<TEntity> GetTrackingExistingSet() =>
        _deletionExpressions.Aggregate(DbContext.Set<TEntity>().AsQueryable(), (current, expr) => current.Where(expr));

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

    public virtual void Add<TDomain>(TDomain domain)
        where TDomain : BaseDomain
    {
        var entity = Mapper.Map<TEntity>(domain);
        DbContext.Set<TEntity>().Add(entity);
        AfterSaveActions.Add(() => domain.Uid = entity.Uid);
    }
    
    public void AddRange<TDomain>(IEnumerable<TDomain> domainSource)
        where TDomain : BaseDomain
    {
        foreach (var domain in domainSource)
            Add(domain);
    }

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