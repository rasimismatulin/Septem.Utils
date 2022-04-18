namespace Septem.Notifications.Core.Config;

public class FcmAppConfiguration
{
    public string PrivateUserSecretKey { get; set; }

    public string InstanceName { get; set; }

    public FcmAppConfiguration(string instanceName, string privateUserSecretKey)
    {
        PrivateUserSecretKey = privateUserSecretKey;
        InstanceName = instanceName;
    }
}