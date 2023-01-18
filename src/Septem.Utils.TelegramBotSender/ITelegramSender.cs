using System;
using System.Threading;
using System.Threading.Tasks;

namespace Septem.Utils.TelegramBotSender;

public interface ITelegramBotSender : IDisposable
{
    Task SendAsync(string body, MessageLevel level, string subject = null, CancellationToken cancellationToken = default);

    Task SendDebugAsync(string body, string subject = null, CancellationToken cancellationToken = default);

    Task SendInformationAsync(string body, string subject = null, CancellationToken cancellationToken = default);

    Task SendWarningAsync(string body, string subject = null, CancellationToken cancellationToken = default);

    Task SendErrorAsync(string body, string subject = null, CancellationToken cancellationToken = default);

    Task SendExceptionAsync(Exception exception, string body = null, string subject = null, CancellationToken cancellationToken = default);
}