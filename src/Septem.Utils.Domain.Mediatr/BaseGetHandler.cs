using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Septem.Utils.Domain.Gateways;
using Septem.Utils.Helpers.ActionInvoke;
using Septem.Utils.MediatR.Features;

namespace Septem.Utils.Domain.Mediatr;

public class BaseGetHandler<TRequest, TDomain, TRepository> : IRequestHandler<TRequest, Result<TDomain>>
    where TRequest : BaseGetFeature<TDomain>
    where TDomain : BaseDomain
    where TRepository : IBaseRepository
{
    protected readonly TRepository Repository;
    protected readonly IMapper Mapper;
    protected readonly IServiceProvider ServiceProvider;


    public BaseGetHandler(TRepository repository, IMapper mapper, IServiceProvider serviceProvider)
    {
        Repository = repository;
        Mapper = mapper;
        ServiceProvider = serviceProvider;
    }


    public async Task<Result<TDomain>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var domain = await Repository.GetAsync<TDomain>(request.Uid, cancellationToken);

        if (domain == default)
            return request.AsNotFound<TDomain>(IssueMessages.ResourceManager.GetString("EntityNotFound", request.CultureInfo));

        return request.AsResultOf(domain);
    }
}