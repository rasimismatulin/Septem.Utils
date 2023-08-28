using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Septem.Utils.Domain.Gateways;
using Septem.Utils.Helpers.ActionInvoke;
using Septem.Utils.Helpers.ActionInvoke.Collection;
using Septem.Utils.MediatR.Features;

namespace Septem.Utils.Domain.Mediatr;

public class BaseGetCollectionHandler<TRequest, TDomain, TRepository, TSearch> : IRequestHandler<TRequest, ResultOfCollection<TDomain>>
    where TSearch : CollectionQuery, new()
    where TRequest : BaseGetCollectionFeature<TDomain, TSearch>
    where TDomain : BaseDomain
    where TRepository : IBaseSearchRepository<TSearch>
{
    protected readonly TRepository Repository;
    protected readonly IMapper Mapper;
    protected readonly IServiceProvider ServiceProvider;

    public BaseGetCollectionHandler(TRepository repository, IMapper mapper, IServiceProvider serviceProvider)
    {
        Repository = repository;
        Mapper = mapper;
        ServiceProvider = serviceProvider;
    }


    public virtual async Task<ResultOfCollection<TDomain>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var domains = await Repository.SearchAsync<TDomain>(request.CollectionQuery, cancellationToken);
        return request.AsResultOfCollection(domains);
    }
}


public class BaseGetCollectionHandler<TRequest, TDomain, TRepository> : IRequestHandler<TRequest, ResultOfCollection<TDomain>>
    where TRequest : BaseGetCollectionFeature<TDomain>
    where TDomain : BaseDomain
    where TRepository : IBaseRepository
{
    protected readonly TRepository Repository;
    protected readonly IMapper Mapper;
    protected readonly IServiceProvider ServiceProvider;

    public BaseGetCollectionHandler(TRepository repository, IMapper mapper, IServiceProvider serviceProvider)
    {
        Repository = repository;
        Mapper = mapper;
        ServiceProvider = serviceProvider;
    }


    public virtual async Task<ResultOfCollection<TDomain>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var domains = await Repository.GetAllAsync<TDomain>(cancellationToken);
        return request.AsResultOfCollection(domains);
    }
}