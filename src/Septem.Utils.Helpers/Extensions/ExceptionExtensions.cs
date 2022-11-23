using System;
using System.Collections.Generic;
using System.Text;

namespace Septem.Utils.Helpers.Extensions;

public static class ExceptionExtensions
{
    public static string GetMessagesAsString(this Exception exception)
    {
        if (exception == null)
            return string.Empty;

        var sb = new StringBuilder();
        while (exception != null)
        {
            sb.AppendLine(exception.Message);
            exception = exception.InnerException;
        }
        return sb.ToString();
    }

    public static IEnumerable<string> GetMessages(this Exception exception)
    {
        while (exception != null)
        {
            yield return exception.Message;
            exception = exception.InnerException;
        }
    }
}