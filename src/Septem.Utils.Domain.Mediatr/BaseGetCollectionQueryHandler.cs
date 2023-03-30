using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Septem.Utils.Domain.Gateways;
using Septem.Utils.Helpers.ActionInvoke;
using Septem.Utils.MediatR.Features;

namespace Septem.Utils.Domain.Mediatr;

public class BaseGetCollectionQueryHandler<TRequest, TDomain, TRepository> : IRequestHandler<TRequest, Result<IQueryable<TDomain>>>
    where TRequest : BaseGetCollectionQueryFeature<TDomain>
    where TDomain : BaseDomain
    where TRepository : IBaseRepository
{
    protected readonly TRepository Repository;
    protected readonly IMapper Mapper;
    protected readonly IServiceProvider ServiceProvider;

    public BaseGetCollectionQueryHandler(TRepository repository, IMapper mapper, IServiceProvider serviceProvider)
    {
        Repository = repository;
        Mapper = mapper;
        ServiceProvider = serviceProvider;
    }

    public virtual Task<Result<IQueryable<TDomain>>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var query = Repository.CollectionQuery<TDomain>();
        return Task.FromResult(request.AsResultOf(query));
    }
}