using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Entities;
using Septem.Notifications.Core.Infrastructure;
using Septem.Notifications.Core.Services.Sender;
using Septem.Notifications.Core.Services.Token;

namespace Septem.Notifications.Core.Services;


internal class NotificationMessageService : INotificationMessageService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly NotificationTokenServiceProvider _notificationTokenServiceProvider;
    private readonly NotificationDbContext _notificationDbContext;
    private readonly NotificationSenderServiceProvider _notificationSenderServiceProvider;
    private readonly ILogger _logger;

    public NotificationMessageService(IServiceProvider serviceProvider, ILoggerFactory loggerFactory,
        NotificationTokenServiceProvider notificationTokenServiceProvider, NotificationDbContext notificationDbContext,
        NotificationSenderServiceProvider notificationSenderServiceProvider)
    {
        _logger = loggerFactory.CreateLogger<NotificationMessageService>();
        _serviceProvider = serviceProvider;
        _notificationTokenServiceProvider = notificationTokenServiceProvider;
        _notificationDbContext = notificationDbContext;
        _notificationSenderServiceProvider = notificationSenderServiceProvider;
    }

    public async Task<ICollection<Guid>> GetPendingNotificationsUidAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationDbContext.Notifications
            .Where(x => x.Status == NotificationStatus.Pending && x.TimeToSendUtc <= DateTimeOffset.UtcNow)
            .OrderBy(x => x.TimeToSendUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.Status = NotificationStatus.WaitReceiversCreation;
            notification.ModifiedDateUtc = DateTimeOffset.UtcNow;
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);

        return notifications.Select(x => x.Uid).ToArray();
    }

    public async Task<ICollection<Guid>> GetNextNotificationsUidAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationDbContext.Notifications
            .Where(x => x.Status == NotificationStatus.ReceiversCreated)
            .OrderBy(x => x.TimeToSendUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.Status = NotificationStatus.WaitMessagesCreation;
            notification.ModifiedDateUtc = DateTimeOffset.UtcNow;
        }
        await _notificationDbContext.SaveChangesAsync(cancellationToken);

        return notifications.Select(x => x.Uid).ToArray();
    }

    public async Task<ICollection<Guid>> GetNextMessagesUidAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var messages = await _notificationDbContext.NotificationMessages
            .Where(x => x.Status == NotificationMessageStatus.Pending)
            .OrderBy(x => x.CreatedDateUtc)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            message.ModifiedDateUtc = DateTimeOffset.UtcNow;
            message.Status = NotificationMessageStatus.Processing;
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);

        return messages.Select(x => x.Uid).ToArray();
    }

    public async Task CreateNotificationReceiversAsync(Guid notificationUid, CancellationToken cancellationToken)
    {
        var notification = await _notificationDbContext.Notifications
            .Include(x => x.Receivers)
            .Where(x => x.Uid == notificationUid)
            .FirstOrDefaultAsync(cancellationToken);

        if (notification == default)
            return;

        var prepareService = _serviceProvider.GetService<INotificationReceiverPrepareService>();

        if (prepareService == null)
        {
            _logger.LogWarning("Couldn't activate INotificationReceiverPrepareService instance to use parameterized receiver");

            notification.Status = NotificationStatus.ReceiversCreated;
            notification.ModifiedDateUtc = DateTimeOffset.UtcNow;

            await _notificationDbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Notification status saved: Uid:{notification.Uid}; Status:{notification.Status};");
            return;
        }


        foreach (var receiverEntity in notification.Receivers)
        {
            if (!string.IsNullOrWhiteSpace(receiverEntity.Parameters))
            {
                var parametersDictionary = JsonSerializer.Deserialize<Dictionary<string, ICollection<string>>>(receiverEntity.Parameters);
                var receiverForPrepare = new Receiver(receiverEntity.ReceiverType, parametersDictionary);
                var receivers = await prepareService.PrepareReceiverAsync(receiverForPrepare, cancellationToken);

                foreach (var receiver in receivers)
                {
                    var newReceiver = Converter.GetReceiverEntity(receiver);
                    notification.Receivers.Add(newReceiver);
                    _logger.LogInformation($"Parameter based receiver added: Type:{newReceiver.ReceiverType}; Token:{newReceiver.Token}; Target:{newReceiver.TargetUid}");
                }
            }
        }
        
        notification.Status = NotificationStatus.ReceiversCreated;
        notification.ModifiedDateUtc = DateTimeOffset.UtcNow;

        await _notificationDbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Notification status saved: Uid:{notification.Uid}; Status:{notification.Status};");
    }


    public async Task CreateNotificationMessagesAsync(Guid notificationUid, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationDbContext.Notifications
            .Include(x => x.Receivers)
            .Where(x => x.Uid == notificationUid)
            .FirstOrDefaultAsync(cancellationToken);

        if (notification == default)
            return;

        foreach (var receiverEntity in notification.Receivers)
        {
            var notificationTokenService = _notificationTokenServiceProvider.GetByReceiverType(receiverEntity.ReceiverType);

            var tokens = await notificationTokenService.GetTokensAsync(receiverEntity, cancellationToken);

            foreach (var notificationTokenEntity in tokens)
            {
                var message = new NotificationMessageEntity
                {
                    NotificationUid = notification.Uid,
                    Title = notification.Title,
                    Payload = notification.Payload
                };

                if (notificationTokenEntity.Uid == default)
                {
                    message.TokenType = notificationTokenEntity.Type;
                    message.Token = notificationTokenEntity.Token;
                }
                else
                {
                    message.NotificationTokenUid = notificationTokenEntity.Uid;
                }

                _notificationDbContext.NotificationMessages.Add(message);
                _logger.LogInformation($"Notification message created: Uid:{notification.Uid}; TargetUid:{message.NotificationTokenUid}; Token: {message.Token}");
            }
        }

        notification.Status = NotificationStatus.MessagesCreated;
        notification.ModifiedDateUtc = DateTimeOffset.UtcNow;

        await _notificationDbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Notification status saved: Uid:{notification.Uid}; Status:{notification.Status};");
    }

    public async Task SendMessagesAsync(Guid messageUid, CancellationToken cancellationToken = default)
    {
        var localizationService = _serviceProvider.GetService<INotificationLocalizationService>();

        var message = await _notificationDbContext.NotificationMessages
            .Where(x => x.Uid == messageUid)
            .Include(x => x.Notification)
            .Include(x => x.NotificationToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (message == default)
            return;

        var notification = Converter.GetNotification(message.Notification);

        if (localizationService != default)
        {
            if (message.NotificationToken != null)
                notification.DefaultLanguage = message.NotificationToken.Language;

            await localizationService.LocalizeNotificationAsync(notification, cancellationToken);

            message.Title = notification.Title;
            message.Payload = notification.Payload;
        }
        else
        {
            _logger.LogWarning("Couldn't activate INotificationLocalizationService instance to localize notifications");
        }

        var token = message.NotificationToken;

        if (token == null)
        {
            if (message.TokenType.HasValue)
            {
                token = new NotificationTokenEntity
                {
                    Type = message.TokenType.Value,
                    Token = message.Token
                };
            }
            else
            {

                _logger.LogError($"Token not provider for this message. Uid:{message.Uid}");

                message.Status = NotificationMessageStatus.Failed;
                message.ExceptionMessage = "Token not provider for this message";
                message.ModifiedDateUtc = DateTimeOffset.UtcNow;
                await _notificationDbContext.SaveChangesAsync(cancellationToken);
                _logger.LogError($"Notification sent failed: Uid:{notification.Uid}");
                return;
            }
        }

        var senderService = _notificationSenderServiceProvider.GetByTokenType(token.Type);
        var result = await senderService.SendAsync(notification, message, token, cancellationToken);

        message.Status = result.IsSuccess ? NotificationMessageStatus.Success : NotificationMessageStatus.Failed;
        message.ServiceMessage = result.ServiceMessage;
        message.ExceptionMessage = result.ExceptionMessage;

        _logger.LogInformation($"Service response: Uid:{notification.Uid}; IsSuccess:{result.IsSuccess}; ServiceMessage:{result.ServiceMessage} ExceptionMessage:{result.ExceptionMessage}");

        message.ModifiedDateUtc = DateTimeOffset.UtcNow;
        await _notificationDbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Notification sent: Uid:{notification.Uid}");
    }

}