using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Septem.Notifications.Core.Infrastructure;

public class NullableDateTimeOffsetToUtcConverter : ValueConverter<DateTimeOffset?, DateTime?>
{
    public NullableDateTimeOffsetToUtcConverter() : base(
        v => v == null ? null : v.Value.UtcDateTime,
        v => v == null ? null : new DateTimeOffset(v.Value, TimeSpan.Zero).ToLocalTime())
    {
    }
}