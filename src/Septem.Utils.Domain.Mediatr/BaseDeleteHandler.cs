using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Septem.Utils.Domain.Gateways;
using Septem.Utils.Helpers.ActionInvoke;
using Septem.Utils.MediatR.Features;

namespace Septem.Utils.Domain.Mediatr;

public class BaseDeleteHandler<TRequest, TDomain, TRepository> : IRequestHandler<TRequest, Result>
      where TRequest : BaseDeleteFeature
      where TDomain : BaseDomain
      where TRepository : IBaseRepository
{
    protected readonly TRepository Repository;
    protected readonly IMapper Mapper;
    protected readonly IServiceProvider ServiceProvider;

    public BaseDeleteHandler(TRepository repository, IMapper mapper, IServiceProvider serviceProvider)
    {
        Repository = repository;
        Mapper = mapper;
        ServiceProvider = serviceProvider;
    }

    public virtual async Task<Result> Handle(TRequest request, CancellationToken cancellationToken)
    {
        await Repository.BeginTransaction(cancellationToken);

        await PreHandle(request, cancellationToken);

        foreach (var uid in request.Uid)
            await Repository.RemoveAsync(uid, cancellationToken);

        await Repository.SaveChangesAsync(cancellationToken);
        await OnSuccess(request, cancellationToken);

        await Repository.CommitTransaction(cancellationToken);

        return request.AsResult();
    }

    public virtual Task PreHandle(TRequest request, CancellationToken cancellationToken) => Task.CompletedTask;

    public virtual Task OnSuccess(TRequest request, CancellationToken cancellationToken) => Task.CompletedTask;
}