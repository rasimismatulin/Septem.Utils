
namespace Septem.Notifications.Core;

public enum NotificationStatus : byte
{
    Pending,
    Canceled,

    WaitReceiversCreation,
    ReceiversCreated,

    WaitMessagesCreation,
    MessagesCreated,

    Failed
}