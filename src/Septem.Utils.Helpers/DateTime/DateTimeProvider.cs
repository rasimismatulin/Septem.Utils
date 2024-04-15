using System;

namespace Septem.Utils.Helpers.DateTime;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;
    
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}