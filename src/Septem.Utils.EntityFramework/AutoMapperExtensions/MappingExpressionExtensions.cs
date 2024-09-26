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
        return option.AfterMap((src, dest, ctx) =>
        {
            new BaseEntityAfterMapAction<TSource, TDestination>().Process(src, dest, ctx);
        });
    }

    public static IMappingExpression<TSource, TDestination> BaseUidEntityAfterMap<TSource, TDestination>(this IMappingExpression<TSource, TDestination> option)
        where TSource : BaseDomain
        where TDestination : BaseEntity
    {
        return option.AfterMap((src, dest, ctx) =>
        {
            new BaseUidEntityAfterMapAction<TSource, TDestination>().Process(src, dest, ctx);
        });
    }
}