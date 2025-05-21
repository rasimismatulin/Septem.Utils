namespace Septem.Notifications.Abstractions.Enums;

public enum NotificationStatus : byte
{
    Pending = 0,
    Canceled = 1,

    WaitReceiversCreation = 2,
    ReceiversCreated = 3,

    WaitMessagesCreation = 4,
    MessagesCreated = 5,

    Failed = 6
}