using System;

namespace Septem.Utils.Helpers.DateTime;

public interface IDateTimeProvider
{
    public DateTimeOffset Now { get; }

    public DateTimeOffset UtcNow { get; }
}