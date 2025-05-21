using Septem.Notifications.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;

namespace Septem.Notifications.Abstractions.Models;

public class Notification
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset TimeToSend { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ModifiedDateUtc { get; set; }

    public bool IsDeleted { get; set; } = false;

    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    public int Type { get; set; }

    public string Title { get; set; }

    public string Payload { get; set; }

    public string Data { get; set; }

    public string DefaultLanguage { get; set; }

    public Guid? GroupKey { get; set; }

    public string CancellationKey { get; set; }

    public Notification()
    {

    }

    public Notification(string payload) : this(null, payload) { }


    public Notification(string title, string payload)
    {
        Title = title;
        Payload = payload;
    }

    public Notification Schedule(DateTimeOffset dateTimeUtc)
    {
        TimeToSend = dateTimeUtc;
        return this;
    }

    public Notification Schedule(TimeSpan timeSpan)
    {
        TimeToSend = DateTimeOffset.UtcNow.Add(timeSpan);
        return this;
    }

    public T GetType<T>()
    {
        return GenerateConvertFrom<int, T>()(Type);
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

    public T GetData<T>()
    {
        return JsonSerializer.Deserialize<T>(Data);
    }

    public Notification SetDefaultLanguage(string defaultLanguage)
    {
        DefaultLanguage = defaultLanguage;
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

    public virtual ICollection<NotificationReceiver> Receivers { get; set; }

    public virtual ICollection<NotificationMessage> Messages { get; set; }


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