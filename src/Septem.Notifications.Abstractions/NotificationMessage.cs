using System;

namespace Septem.Notifications.Abstractions;

public class NotificationMessage
{
    public Guid NotificationUid { get; set; }
    public Guid MessageUid { get; set; }

    public string Title { get; set; }

    public string Payload { get; set; }

    public DateTime CreatedDateUtc { get; set; }
    public DateTime SentDateUtc { get; set; }

    public DateTime CreatedDate => CreatedDateUtc.ToLocalTime();
    public DateTime SentDate => SentDateUtc.ToLocalTime();

    public NotificationTokenType TokenType { get; set; }

    public bool IsSuccess { get; set; }

    public bool IsView { get; set; }
}