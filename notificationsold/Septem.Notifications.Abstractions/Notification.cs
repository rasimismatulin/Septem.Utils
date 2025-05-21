using System;
using System.Linq.Expressions;
using System.Text.Json;

namespace Septem.Notifications.Abstractions;

public class Notification
{
    public Guid Uid { get; set; }

    public byte Type { get; set; }

    public string Title { get; set; }

    public string Payload { get; set; }

    public string Data { get; set; }

    public DateTimeOffset TimeToSendUtc { get; set; } = DateTimeOffset.UtcNow;

    public string DefaultLanguage { get; set; }

    public Guid? GroupKey { get; set; }

    public string CancellationKey { get; set; }

    public FcmConfiguration FcmConfiguration { get; set; }

    public Notification()
    {

    }

    public Notification(string payload) : this(default, payload) { }


    public Notification(string title, string payload)
    {
        Title = title;
        Payload = payload;
    }

    public Notification SetType<T>(T enumType)
    {
        Type = GenerateConvertTo<T, byte>()(enumType);
        return this;
    }

    public Notification SetData(object data)
    {
        Data = JsonSerializer.Serialize(data);
        return this;
    }


    public Notification SetDefaultLanguage(string defaultLanguage)
    {
        DefaultLanguage = defaultLanguage;
        return this;
    }

    public Notification Schedule(DateTimeOffset dateTimeUtc)
    {
        TimeToSendUtc = dateTimeUtc;
        return this;
    }


    public Notification SetGroupKey(Guid groupKey)
    {
        GroupKey = groupKey;
        return this;
    }

    public Notification SetCancellationKey(string cancellationKey)
    {
        CancellationKey = cancellationKey;
        return this;
    }

    public T GetType<T>()
    {
        return GenerateConvertFrom<byte, T>()(Type);
    }

    public T GetData<T>()
    {
        return JsonSerializer.Deserialize<T>(Data);
    }

    public Notification SetFcmConfiguration(FcmConfiguration fcmConfiguration)
    {
        FcmConfiguration = fcmConfiguration;
        return this;
    }

    private static Func<TEnum, TResult> GenerateConvertTo<TEnum, TResult>()
        where TResult : struct, IComparable, IFormattable, IConvertible, IComparable<TResult>, IEquatable<TResult>
    {
        var value = Expression.Parameter(typeof(TEnum));
        var ue = Expression.Convert(value, typeof(TResult));
        return Expression.Lambda<Func<TEnum, TResult>>(ue, value).Compile();
    }

    private static Func<TY, T> GenerateConvertFrom<TY, T>()
        where TY : struct, IComparable, IFormattable, IConvertible, IComparable<TY>, IEquatable<TY>
    {
        var value = Expression.Parameter(typeof(TY));
        var ue = Expression.Convert(value, typeof(T));
        return Expression.Lambda<Func<TY, T>>(ue, value).Compile();
    }
}