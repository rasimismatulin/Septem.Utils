using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using Septem.Notifications.Core.Services.Sender;

namespace Septem.Notifications.Core.Config;

public class FcmConfigurationBuilder
{
    private readonly IServiceCollection _services;

    public FcmConfigurationBuilder(IServiceCollection services)
    {
        _services = services;
    }
    public FcmConfigurationBuilder Default()
    {
        _services.AddTransient<FcmNotificationSenderService>();
        return this;
    }

    public void AddInstance(string privateUserSecretKey)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(privateUserSecretKey)
        }, "Default");
    }

    public void AddInstance(string instanceName, string privateUserSecretKey)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(privateUserSecretKey)
        }, instanceName);
    }
}