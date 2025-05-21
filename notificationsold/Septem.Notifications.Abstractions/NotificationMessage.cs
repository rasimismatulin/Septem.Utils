using System;

namespace Septem.Notifications.Abstractions;

public class NotificationMessage
{
    public Guid NotificationUid { get; set; }
    public Guid MessageUid { get; set; }

    public string Title { get; set; }

    public string Payload { get; set; }

    public DateTimeOffset CreatedDateUtc { get; set; }
    public DateTimeOffset SentDateUtc { get; set; }
    
    public NotificationTokenType TokenType { get; set; }

    public bool IsSuccess { get; set; }

    public bool IsView { get; set; }
}