using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Septem.Utils.Helpers.ActionInvoke.Collection;

namespace Septem.Utils.Domain.Gateways;

public interface IBaseSearchRepository<in TSearch> : IBaseRepository
    where TSearch : CollectionQuery
{

    Task<bool> ExistsAsync(TSearch search, CancellationToken cancellationToken);

    Task<ICollection<TDomain>> SearchAsync<TDomain>(TSearch search, CancellationToken cancellationToken)
        where TDomain : BaseDomain;

    Task<TDomain> SearchFirstAsync<TDomain>(TSearch search, CancellationToken cancellationToken)
        where TDomain : BaseDomain;
}