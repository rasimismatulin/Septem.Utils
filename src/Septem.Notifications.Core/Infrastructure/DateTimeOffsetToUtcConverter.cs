using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Septem.Notifications.Core.Infrastructure;

public class DateTimeOffsetToUtcConverter : ValueConverter<DateTimeOffset, DateTime>
{
    public DateTimeOffsetToUtcConverter() : base(
        v => v.UtcDateTime,
        v => new DateTimeOffset(v, TimeSpan.Zero).ToLocalTime())
    {
    }
}