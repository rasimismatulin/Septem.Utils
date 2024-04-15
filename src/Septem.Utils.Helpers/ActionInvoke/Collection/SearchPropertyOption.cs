using System;
using Septem.Utils.Helpers.ActionInvoke.Collection.Enums;

namespace Septem.Utils.Helpers.ActionInvoke.Collection;

public class SearchPropertyOption<T> : SearchPropertyOption
{
    public T Value { get; set; }

    public SearchPropertyOption()
    {
    }

    public SearchPropertyOption(JoinOptions joinOption, SearchOptions searchOption)
    {
        JoinOption = joinOption;
        SearchOption = searchOption;
    }

    public void Set(T value)
    {
        Value = value;
    }

    public void SetLike(T value)
    {
        Value = value;
        SearchOption = SearchOptions.Like;
    }

    public void SetAnd(T value)
    {
        Value = value;
        JoinOption = JoinOptions.And;
    }

    public void SetOr(T value)
    {
        Value = value;
        JoinOption = JoinOptions.Or;
    }

    public void SetAndDefault()
    {
        IsSetDefault = true;
        Value = default;
        JoinOption = JoinOptions.And;
    }

    public void SetOrDefault()
    {
        IsSetDefault = true;
        Value = default;
        JoinOption = JoinOptions.Or;
    }

    public bool IsSetDefault { get; set; }

    public override string GetStringValue()
    {
        if (Value == null)
            return null;
        if (Value is Enum e)
        {
            var b = (byte)(Value as object);
            return b.ToString();
        }
        if (Value is System.DateTime DateTime)
        {
            return DateTime.ToString("yyyy-MM-dd");
        }
        return Value.ToString();
    }

    public override bool CanApply
    {
        get
        {
            if (IsSetDefault)
                return true;
            if (!Equals(Value, default(T)))
                return !string.IsNullOrWhiteSpace(GetStringValue());
            return false;
        }
    }

    public override object GetValue() => Value;
}

public abstract class SearchPropertyOption
{
    public JoinOptions JoinOption { get; protected set; }

    public SearchOptions SearchOption { get; set; }

    public abstract string GetStringValue();

    public abstract bool CanApply { get; }

    public abstract object GetValue();
}