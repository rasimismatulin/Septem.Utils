# Septem Notification Server 

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

Septem Notification Server allow you to create Notifications in you project and send them in one line of code

## Features

- Sms notifications
- Email notifications
- Firebase notifications
- Unlimited receivers
- Localize notifications for each receiver
- Parameterlized receivers

## Installation

Library requires [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to run.
### Install on domain or other notification created library
```sh
PM> Install-Package Septem.Notifications.Abstractions
```
### Install on ASP NET Core Web App
```sh
PM> Install-Package Septem.Notifications.Core
```
Add relations for EF and Postgres Database
```sh
PM> Install-Package Microsoft.EntityFrameworkCore
PM> Install-Package Microsoft.EntityFrameworkCore.Relational
PM> Install-Package Npgsql
PM> Install-Package Npgsql.EntityFrameworkCore.PostgreSQL
```
Suggested logger lib
```sh
PM> Install-Package Serilog
PM> Install-Package Serilog.Extensions.Logging
```
Register services in ConfigureServices method
```sh
services.AddNotifications(options =>
{
    options.UseLoggerFactory(new SerilogLoggerFactory());
    options.UseNpgsql("conenctionString", b => b.MigrationsAssembly("Septem.Notifications.Core"));
});
```
Add this code in Configure method
```sh
app.UseNotifications();
```
### Install on Worker app
```sh
PM> Install-Package Septem.Notifications.Core
PM> Install-Package Septem.Notifications.Jobs
```
Register services in ConfigureServices method
```sh
services.AddNotificationsForWorker(options =>
{
    options.UseLoggerFactory(new SerilogLoggerFactory());
    options.UseNpgsql("conenctionString", b => b.MigrationsAssembly("Septem.Notifications.Core"));
});
services.AddNotificationJobs();
```
Inject JobScheduler and start
```sh
private readonly JobScheduler _jobScheduler;
public Worker(JobScheduler jobScheduler)
{
    _jobScheduler = jobScheduler;
}
public override async Task StartAsync(CancellationToken cancellationToken)
{
    await _jobScheduler.StartAsync();
}
```

## Usage

## Injecion
```sh
private readonly INotificationTokenService _tokenService;
private readonly INotificationService _service;
public IndexModel(INotificationTokenService tokenService, INotificationService service)
{
    _tokenService = tokenService;
    _service = service;
}
```

## Quick notification in one line
```sh
await _service.CreateNotificationAsync(new Notification("OTP code: 12432"), Receiver.Sms().WithToken("994503624553"), CancellationToken.None);
```

## Create advance notification 
1. Notification with title and payload
```sh
var notification = new Notification("My title", "My payload");
```
2. If you need to separate Notifications by type in your localization service you can specify notification type with your custom enum
```sh    
public enum MyNotificationType 
{    
    MyNotificationType,
    OtherNotificationType
}
notification.SetType(MyNotificationType.MyNotificationType);
```
3. If you need to add additional data in notification object
```sh  
public class MyNotificationData
{
    public string Data1 { get; set; }
    public Guid Data2 { get; set; }
}
notification.SetData(new MyNotificationData { Data1 = "My data 1", Data2 = Guid.NewGuid() });
```
4. If you want to specify default notification language
```sh
notification.SetDefaultLanguage("az");
```
5. Also you can write it in one code line
```sh
var notification = new Notification("My title", "My payload")
    .SetType(MyNotificationType.MyNotificationType)
    .SetData(new MyNotificationData { Data1 = "My data 1", Data2 = Guid.NewGuid() })
    .SetDefaultLanguage("az");
```
6. If notification will localize in service you can create notification with empty title and payload
```sh
var notification = new Notification().SetType(MyNotificationType.MyNotificationType);
```
7. For sms notififcation title will be ignore and you can create notification with payload only
```sh
var notification = new Notification("My notification payload");
```
8. Schedule notifications
```sh
var scheduledNotification = new Notification("My title", "My payload").Schedule(DateTime.Now.AddMinutes(5));
var scheduledNotificationUtc = new Notification("My title", "My payload").ScheduleUtc(DateTime.UtcNow.AddMinutes(5));
```

## Cancelling notification
```sh
var groupUid = Guid.NewGuid();
var cancellationKey = "someKeyToCancelNotifications";
var existingNotificationUid = Guid.NewGuid();

var notificationGroup = new Notification("My title 1", "My payload 1")
    .Schedule(DateTime.Now.AddHours(1))
    .SetGroupKey(groupUid);

var notificationCancellation = new Notification("My title 2", "My payload 2")
    .Schedule(DateTime.Now.AddHours(1))
    .SetCancellationKey(cancellationKey);

await _service.CancelByUidAsync(existingNotificationUid, CancellationToken.None);
await _service.CancelAsync(groupUid, CancellationToken.None);
await _service.CancelAsync(cancellationKey, CancellationToken.None);
```

## Notification message history
```sh
var userUid = Guid.Parse("6c54be37-8f0f-8096-491a-8b5f37ff440e");
var history = await _notificationMessageHistoryService.GetNotificationHistory(userUid, CancellationToken.None);
```

## Create notification token
Notification tokens use to save you system use tokens. There are 3 type of tokens, Sms, Email and Fcm.
Tokens are unique per user, type and deviceId

1. Create tokens with any type. Fcm token types can use userDeviceId parameter. (In principle, all types of tokens can have this parameter, but I don’t know why you might need this in a real project). Then UserUid parameter will explain as TargetUid.
```sh
var smsToken = NotificationToken.Sms(userUid, "994503624553", "az");
var emailToken = NotificationToken.Email(userUid, "rasim.ismatulin@gmail.com", "az");
var fcmToken = NotificationToken.Fcm(userUid, "==eweqfr4rflfre/ertwert345=3", "az");
var fcmWithDeviceToken = NotificationToken.FcmWithDevice(userUid, "=34gg5opi343if030", "userDeviceId", "az");
```
2. Save tokens. For each token in collection service vwill check if token with given targetUid, tokenType, and deviceId(if provided in model) exsist. If token exists in DB then it will be updated else created new one.
```sh
await _tokenService.SaveAsync(new [] { smsToken, emailToken, fcmToken, fcmWithDeviceToken }, CancellationToken.None);
```
3. You can manualy delete tokens. For example on user logout, blocked and etc. 
Note: if deviceid is not provided then sevice will delete all target token with provided token type;
      if you need to dele only token with empty deviceid. Then send default(string) as deviceId
```sh
await _tokenService.RemoveAsync(userUid, NotificationTokenType.Sms, CancellationToken.None);
await _tokenService.RemoveAsync(userUid, NotificationTokenType.Email, CancellationToken.None);
await _tokenService.RemoveAsync(userUid, NotificationTokenType.Fcm, "userDeviceId222", CancellationToken.None);
await _tokenService.RemoveAsync(userUid, NotificationTokenType.Fcm, CancellationToken.None);
```
4. Get token information
```sh
var checkToken1 = await _tokenService.GetAsync(userUid, NotificationTokenType.Sms, CancellationToken.None);
var checkToken2 = await _tokenService.GetAsync(userUid, NotificationTokenType.Email, CancellationToken.None);
var checkToken3 = await _tokenService.GetAsync(userUid, NotificationTokenType.Fcm, CancellationToken.None);
var checkToken4 = await _tokenService.GetAsync(userUid, NotificationTokenType.Fcm, "userDeviceId222", CancellationToken.None);
```
## Create receivers 
There are 4 type of receivers
```sh
var smsReceiver = Receiver.Sms();
var emailReceiver = Receiver.Email();
var fcmReceiver = Receiver.Fcm();
var fcmOrSmsReceiver = Receiver.FcmOrSms();
```
FcmOrSms receiver will work only with specified TargetUid (will discuss later) first try to find FCM token and send notification. If fcm token not found then try to send sms token 
1. Receiver with token. Sms notification will send  to provided token (phone number)
```sh
var withTokenSmsReceiver = Receiver.Sms().WithToken("994503624553");
var withTokenEmailReceiver = Receiver.Email().WithToken("rasim.ismatulin@gmail.com");
var withTokenFcmReceiver = Receiver.Fcm().WithToken("==234rere/324reflw\f");
```
2. Receiver with tokens. Notification will send  to all provided tokens.
```sh
var withTokensSmsReceiver = Receiver.Sms().WithTokens("994503624553", "994553624553");
```
3. Receiver with target. Notification will send to all tokens which exists in database for provided targetUid
```sh
var withTargetSmsReceiver = Receiver.Sms().WithTarget(userUid);
```
3. Receiver with target. Notification will send to all tokens which exists in database for provided targetUid
```sh
var userUid = Guid.Parse("65594d44-82db-2a83-412d-0c6d137ff2cc");
var withTargetSmsReceiver = Receiver.Sms().WithTarget(userUid);
```
4. Receiver with targets. Notification will send to all tokens which exists in database for provided each targetUid
```sh
var userUid1 = Guid.Parse("65594d44-82db-2a83-412d-0c6d137ff2cc");
var userUid2 = Guid.Parse("65594d44-82db-2a83-412d-0c6d137ff2cc");
var withTargetsSmsReceiver = Receiver.Sms().WithTargets(userUid1, userUid2);
```

4. Receiver with parameters. First service will call INotificationReceiverPrepareService.PrepareReceiverAsync method, which you need to implement in you Worker Service. Then notification will send to all returned receivers.
```sh
var withParameterSmsReceiver = Receiver.Sms().WithParameter("userAddress", "56").WithParameters("topicName", "My name", "My 1 name", "Name 2 name");
```


## Save notification (finally)
```sh            
var receivers = new[] { smsReceiver, emailReceiver, fcmReceiver, fcmOrSmsReceiver };
await _service.CreateNotificationAsync(notification, receivers, CancellationToken.None);
```

## Notification Localization
### Notification token language update. 
```sh
await _tokenService.SaveLanguageAsync("ru", userUid, NotificationTokenType.Sms, CancellationToken.None);
await _tokenService.SaveLanguageAsync("ru", userUid, NotificationTokenType.Fcm, "userDeviceId", CancellationToken.None);
```

### Notification Default Language
```sh
var localizedNotification = new Notification()
    .SetType(MyNotificationType.OtherNotificationType)
    .SetDefaultLanguage("az");

await _service.CreateNotificationAsync(localizedNotification, Receiver.Sms().WithTarget(userUid), CancellationToken.None);
```

### Implement INotificationLocalizationService interface on worker app
1. Register  INotificationLocalizationService startup.cs
```sh
    services.AddScoped<INotificationLocalizationService, NotificationLocalizationService>();
```
2. Localize string via static resource file or get data from DB
```sh
    internal class NotificationLocalizationService : INotificationLocalizationService
    {
        public async Task LocalizeNotificationAsync(Notification notification, CancellationToken cancellationToken)
        {
            var type = notification.GetType<MyNotificationType>();
            var ci = new CultureInfo(notification.DefaultLanguage);
            switch (type)
            {
                case MyNotificationType.MyNotificationType:
                    await LocalizeMyNotificationType(notification, ci);
                    break;
                case MyNotificationType.OtherNotificationType:
                    break;
                default:
                    notification.Title = Strings.ResourceManager.GetString($"{type}Title", ci);
                    notification.Payload = Strings.ResourceManager.GetString($"{type}Payload", ci);
                    break;
            }
        }

        private Task LocalizeMyNotificationType(Notification notification, CultureInfo ci)
        {
            var data = notification.GetData<MyNotificationData>();

            var data1 = data.Data1;
            var data2 = data.Data2;

            //Can get this data from DB for example

            notification.Title = data1 + "Some title from DB";
            notification.Payload = data2 + "Some payload from DB";

            return Task.CompletedTask;
        }
    }
```


## Implement Receivers with parameters
1. Register  INotificationReceiverPrepareService startup.cs 
```sh
services.AddScoped<INotificationReceiverPrepareService, NotificationReceiverPrepareService>();
```

2. Get static receivers or get data from DB
```sh
public class NotificationReceiverPrepareService : INotificationReceiverPrepareService
{
    public Task<ICollection<Receiver>> PrepareReceiverAsync(Receiver receiver, CancellationToken cancellationToken)
    {
        var param1 = receiver.GetParameter("userAddress");
        var param2 = receiver.GetParameters("topicName");

        //Get receiver from DB pr other service

        return Task.FromResult(Receiver.Sms().WithTokens("994503624553"));
    }
}
```

## License

MIT

**Free Software, Hell Yeah!**

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job. There is no need to format nicely because it shouldn't be seen. Thanks SO - http://stackoverflow.com/questions/4823468/store-comments-in-markdown-syntax)

   [dill]: <https://github.com/joemccann/dillinger>
   [git-repo-url]: <https://github.com/joemccann/dillinger.git>
   [john gruber]: <http://daringfireball.net>
   [df1]: <http://daringfireball.net/projects/markdown/>
   [markdown-it]: <https://github.com/markdown-it/markdown-it>
   [Ace Editor]: <http://ace.ajax.org>
   [node.js]: <http://nodejs.org>
   [Twitter Bootstrap]: <http://twitter.github.com/bootstrap/>
   [jQuery]: <http://jquery.com>
   [@tjholowaychuk]: <http://twitter.com/tjholowaychuk>
   [express]: <http://expressjs.com>
   [AngularJS]: <http://angularjs.org>
   [Gulp]: <http://gulpjs.com>

   [PlDb]: <https://github.com/joemccann/dillinger/tree/master/plugins/dropbox/README.md>
   [PlGh]: <https://github.com/joemccann/dillinger/tree/master/plugins/github/README.md>
   [PlGd]: <https://github.com/joemccann/dillinger/tree/master/plugins/googledrive/README.md>
   [PlOd]: <https://github.com/joemccann/dillinger/tree/master/plugins/onedrive/README.md>
   [PlMe]: <https://github.com/joemccann/dillinger/tree/master/plugins/medium/README.md>
   [PlGa]: <https://github.com/RahulHP/dillinger/blob/master/plugins/googleanalytics/README.md>
