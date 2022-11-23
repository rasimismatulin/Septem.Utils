using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.SmartEnum;

namespace Septem.Utils.Helpers;

public abstract class SmartEnumBase<TEnum, TValue> : SmartEnum<TEnum, TValue>
    where TEnum : SmartEnumBase<TEnum, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    protected SmartEnumBase(string name, TValue value) : base(name, value)
    {
    }

    public bool CanTransitionTo(TEnum next) => GetTransitions().Any(x => x == next);

    public virtual IEnumerable<TEnum> GetTransitions() => Enumerable.Empty<TEnum>();

    public static IEnumerable<object> ListWithTransitions(Func<string, string> localize)
    {
        return List.Select(x => new
        {
            x.Name,
            x.Value,
            Caption = localize(x.Name),
            Transitions = x.GetTransitions().OrderBy(t => t.Value).Select(t => new
            {
                t.Name,
                t.Value,
                Caption = localize(t.Name),
            })
        }).OrderBy(x => x.Value);
    }
}

