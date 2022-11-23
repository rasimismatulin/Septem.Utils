using System;
using AutoMapper;
using Septem.Utils.EntityFramework.Entities;

namespace Septem.Utils.EntityFramework.AutoMapperExtensions;

public class BaseUidEntityAfterMapAction<TSource, TDestination> : IMappingAction<TSource, TDestination>
    where TDestination : BaseEntity
{
    public void Process(TSource source, TDestination destination, ResolutionContext context)
    {
        if (destination.Uid == default)
        {
            destination.Uid = Guid.NewGuid();
        }
    }
}