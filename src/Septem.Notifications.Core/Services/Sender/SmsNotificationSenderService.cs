using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Config;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Services.Sender;

internal class SmsNotificationSenderService : INotificationSenderService
{
    public async Task<SentStatus> SendAsync(Notification notification, NotificationMessageEntity message, NotificationTokenEntity token, CancellationToken cancellationToken)
    {
        try
        {
            var messageUrl = SmsOptionBuilder.SmsOptionProvider.CreateMessageUrl(token.Token, message.Payload);

            var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, messageUrl)
            {

            }, cancellationToken);

            var responseStream = await result.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(responseStream);
            var content = await reader.ReadToEndAsync();

            return content.Contains("errtext=OK") ?
                SentStatus.Success(content) :
                SentStatus.Fail(content);
        }
        catch (Exception e)
        {
            return SentStatus.Fail(e);
        }
    }
}