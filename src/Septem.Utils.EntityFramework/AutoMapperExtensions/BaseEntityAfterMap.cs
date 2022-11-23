using System;
using AutoMapper;
using Septem.Utils.EntityFramework.Entities;

namespace Septem.Utils.EntityFramework.AutoMapperExtensions;

public class BaseEntityAfterMapAction<TSource, TDestination> : IMappingAction<TSource, TDestination>
    where TDestination : BasePersistenceEntity
{
    public void Process(TSource source, TDestination destination, ResolutionContext context)
    {
        if (destination.Uid == default)
        {
            destination.Uid = Guid.NewGuid();

            if (destination.CreatedUtc == default)
                destination.CreatedUtc = DateTime.UtcNow;

            destination.IsDeleted = false;
            destination.ModifiedUtc = default;
        }
        else
        {
            destination.ModifiedUtc = DateTime.UtcNow;
        }
    }
}