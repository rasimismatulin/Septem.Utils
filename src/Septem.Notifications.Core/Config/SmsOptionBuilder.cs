namespace Septem.Notifications.Core.Config;

public class SmsOptionBuilder
{
    public static SmsUrlProvider SmsOptionProvider;

    public string ApiEndpoint { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string SenderName { get; set; }

    public void Build()
    {
        SmsOptionProvider = new SmsUrlProvider(ApiEndpoint, UserName, Password, SenderName);
    }
}