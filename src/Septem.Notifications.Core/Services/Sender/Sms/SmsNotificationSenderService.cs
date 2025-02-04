using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Config;
using Septem.Notifications.Core.Entities;

namespace Septem.Notifications.Core.Services.Sender.Sms;

internal class SmsNotificationSenderService : INotificationSenderService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public SmsNotificationSenderService(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<SentStatus> SendAsync(Notification notification, NotificationMessageEntity message, 
        NotificationTokenEntity token, CancellationToken cancellationToken)
    {
        try
        {
            var url = BuildUrl(notification, token.Token, message.Payload);

            using var httpClient = _httpClientFactory.CreateClient(nameof(SmsNotificationSenderService));
            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            var isSuccess = Validate(notification, responseMessage);

            return isSuccess ? SentStatus.Success(responseMessage) : SentStatus.Fail(responseMessage);
        }
        catch (Exception e)
        {
            return SentStatus.Fail(e);
        }
    }
    public virtual string BuildUrl(Notification notification, string token, string payload) =>
        SmsOptionBuilder.UrlBuilder(_serviceProvider, notification, token, payload);

    public virtual bool Validate(Notification notification, string responseMessage) =>
        SmsOptionBuilder.Validator(_serviceProvider, notification, responseMessage);
}