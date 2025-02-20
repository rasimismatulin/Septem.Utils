using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Septem.Notifications.Abstractions;
using Septem.Notifications.Core.Entities;
using Septem.Notifications.Core.Infrastructure;

namespace Septem.Notifications.Core.Services;

internal class NotificationService : INotificationService
{
    private readonly NotificationDbContext _notificationDbContext;
    private readonly ILogger _logger;

    public NotificationService(ILogger<NotificationService> logger, NotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
        _logger = logger;
    }


    public async Task CreateNotificationAsync(Notification notification, Receiver receiver, CancellationToken cancellationToken)
    {
        await CreateNotificationAsync(notification, [receiver], cancellationToken);
    }

    public async Task CreateNotificationAsync(Notification notification, ICollection<Receiver> receivers, CancellationToken cancellationToken)
    {
        var groupKey = Guid.NewGuid();

        if (!receivers.Any())
        {
            _logger.LogWarning($"Notification have no receivers! Uid:{groupKey}; Type:{notification.Type}; Title:{notification.Title}");
            return;
        }

        var notificationModel = Converter.GetNotificationEntity(notification);

        var scheduled = notificationModel.TimeToSendUtc > DateTimeOffset.UtcNow;
        var hasParameters = receivers.Any(x => x.Parameters.Any());
        var hasTargets = receivers.Any(x => x.TargetUid.HasValue);


        if (!scheduled && !hasParameters)
            notificationModel.Status = hasTargets ? NotificationStatus.ReceiversCreated : NotificationStatus.MessagesCreated;

        _logger.LogInformation($"Parameters: Uid:{groupKey}; Scheduled:{scheduled}; HasParameters:{hasParameters}; HasTargets:{hasTargets}; Status: {notificationModel.Status}");

        foreach (var receiver in receivers)
        {
            if (!scheduled && !hasParameters && !hasTargets)
            {
                var message = new NotificationMessageEntity
                {
                    Token = receiver.Token,
                    Title = notification.Title,
                    Payload = notification.Payload,
                    TokenType = Converter.ToTokenType(receiver.ReceiverType)
                };
                notificationModel.Messages.Add(message);
                _logger.LogInformation($"Message created. Uid:{groupKey}; Title:{notification.Title}; Token:{message.Token}");
            }

            var receiverEntity = Converter.GetReceiverEntity(receiver);
            notificationModel.Receivers.Add(receiverEntity);
            _logger.LogInformation($"Receiver created. Uid:{groupKey}; Title: {receiverEntity.ReceiverType};");
        }

        _notificationDbContext.Notifications.Add(notificationModel);
        await _notificationDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Notification saved. Uid:{groupKey};");
    }

    public async Task CancelAsync(Guid groupKey, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationDbContext.Notifications
            .Where(x => x.Status == NotificationStatus.Pending && x.GroupKey == groupKey)
            .ToArrayAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.Status = NotificationStatus.Canceled;
            _logger.LogInformation($"Notification canceled. Group key:{groupKey}; Notification:{notification.Uid};");
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelAsync(string cancellationKey, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationDbContext.Notifications
            .Where(x => x.Status == NotificationStatus.Pending && x.CancellationKey == cancellationKey)
            .ToArrayAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.Status = NotificationStatus.Canceled;
            _logger.LogInformation($"Notification canceled. Cancellation key:{cancellationKey}; Notification:{notification.Uid};");
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelByUidAsync(Guid uid, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationDbContext.Notifications
            .Where(x => x.Status == NotificationStatus.Pending && x.Uid == uid)
            .ToArrayAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.Status = NotificationStatus.Canceled;
            _logger.LogInformation($"Notification canceled. Notification:{notification.Uid};");
        }

        await _notificationDbContext.SaveChangesAsync(cancellationToken);

    }

    public void Dispose()
    {
        _notificationDbContext?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_notificationDbContext != null) await _notificationDbContext.DisposeAsync();
    }
}