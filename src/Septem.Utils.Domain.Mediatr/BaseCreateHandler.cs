using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Septem.Utils.Domain.Gateways;
using Septem.Utils.Helpers.ActionInvoke;
using Septem.Utils.MediatR.Features;

namespace Septem.Utils.Domain.Mediatr;

public class BaseCreateHandler<TRequest, TDomain, TRepository> : IRequestHandler<TRequest, Result<TDomain>>
    where TRequest : BaseCreateFeature<TDomain>
    where TDomain : BaseDomain
    where TRepository : IBaseRepository
{
    protected readonly TRepository Repository;
    protected readonly IMapper Mapper;
    protected readonly IServiceProvider ServiceProvider;
    

    public BaseCreateHandler(TRepository repository, IMapper mapper, IServiceProvider serviceProvider)
    {
        Repository = repository;
        Mapper = mapper;
        ServiceProvider = serviceProvider;
    }
    
    public virtual async Task<Result<TDomain>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var domain = Mapper.Map<TDomain>(request);

        await Repository.BeginTransaction(cancellationToken);

        await PreHandleAsync(request, domain, cancellationToken);
        Repository.Add(domain);
        await Repository.SaveChangesAsync(cancellationToken);
        await OnSuccessAsync(request, domain, cancellationToken);

        await Repository.CommitTransaction(cancellationToken);

        return request.AsResultOf(domain);
    }

    public virtual Task PreHandleAsync(TRequest request, TDomain domain, CancellationToken cancellationToken) => Task.CompletedTask;

    public virtual Task OnSuccessAsync(TRequest request, TDomain domain, CancellationToken cancellationToken) => Task.CompletedTask;
}