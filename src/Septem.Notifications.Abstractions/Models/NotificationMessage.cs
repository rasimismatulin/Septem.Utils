using System;
using Septem.Notifications.Abstractions.Enums;

namespace Septem.Notifications.Abstractions.Models;

public class NotificationMessage
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ModifiedDateUtc { get; set; }

    public bool IsDeleted { get; set; } = false;

    public NotificationMessageStatus Status { get; set; } = NotificationMessageStatus.Pending;

    public string Title { get; set; }

    public string Payload { get; set; }

    public string Token { get; set; }

    public ReceiverType? TokenType { get; set; }

    public string ServiceMessage { get; set; }

    public string ExceptionMessage { get; set; }

    public bool IsView { get; set; }
}