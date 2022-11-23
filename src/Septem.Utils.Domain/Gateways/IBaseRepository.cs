using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.Utils.Domain.Gateways;

public interface IBaseRepository
{
    Task WithTransaction(Func<Task> action, CancellationToken cancellationToken);

    Task BeginTransaction(CancellationToken cancellationToken);

    Task CommitTransaction(CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    IQueryable<TDomain> CollectionQuery<TDomain>();

    Task<bool> ExistsAsync(Guid uid, CancellationToken cancellationToken);

    Task RemoveAsync(Guid uid, CancellationToken cancellationToken);

    Task<TDomain> GetAsync<TDomain>(Guid uid, CancellationToken cancellationToken)
        where TDomain : BaseDomain;

    Task<ICollection<TDomain>> GetAllAsync<TDomain>(CancellationToken cancellationToken)
        where TDomain : BaseDomain;

    Task AddAsync<TDomain>(TDomain entity, CancellationToken cancellationToken)
        where TDomain : BaseDomain;

    Task UpdateAsync<TDomain>(TDomain entity, CancellationToken cancellationToken)
        where TDomain : BaseDomain;
}
