using System.Collections.Generic;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Septem.Notifications.Core.Config;

public class FcmConfigurationBuilder
{
    private readonly List<FcmAppConfiguration> _configurations;

    public FcmConfigurationBuilder()
    {
        _configurations = new List<FcmAppConfiguration>();
    }

    public void AddInstance(string privateUserSecretKey)
    {
        _configurations.Add(new FcmAppConfiguration("Default", privateUserSecretKey));
    }

    public void AddInstance(string instanceName, string privateUserSecretKey)
    {
        _configurations.Add(new FcmAppConfiguration(instanceName, privateUserSecretKey));
    }

    public void Build()
    {
        foreach (var fcmAppConfiguration in _configurations)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(fcmAppConfiguration.PrivateUserSecretKey)
            }, fcmAppConfiguration.InstanceName);
        }
    }
}