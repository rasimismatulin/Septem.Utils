using System;
using System.Text;

namespace Septem.Notifications.Core.Services.Sender;

internal class SentStatus
{
    public bool IsSuccess { get; set; }

    public string ServiceMessage { get; set; }

    public string ExceptionMessage { get; set; }

    public Exception Exception { get; set; }

    public static SentStatus Success(string serviceMessage = default) => new()
    {
        IsSuccess = true,
        ServiceMessage = serviceMessage
    };

    public static SentStatus Fail(string serviceMessage) => new()
    {
        IsSuccess = false,
        ServiceMessage = serviceMessage
    };

    public static SentStatus Fail(Exception exception) => new()
    {
        IsSuccess = false,
        Exception = exception,
        ExceptionMessage = GetExceptionMessages(exception)
    };

    private static string GetExceptionMessages(Exception exception)
    {
        var sb = new StringBuilder();
        while (exception != null)
        {
            sb.Append(exception.Message);
            exception = exception.InnerException;
        }
        return sb.ToString();
    }
}