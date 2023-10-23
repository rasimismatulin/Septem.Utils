using AutoMapper;
using Septem.Utils.Domain;
using Septem.Utils.EntityFramework.Entities;

namespace Septem.Utils.EntityFramework.AutoMapperExtensions;

public static class MappingExpressionExtensions
{
    public static IMappingExpression<TSource, TDestination> BaseEntityAfterMap<TSource, TDestination>(this IMappingExpression<TSource, TDestination> option)
        where TSource : BaseDomain
        where TDestination : BasePersistenceEntity
    {
        return option
            .ForMember(dest => dest.CreatedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedUtc, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}