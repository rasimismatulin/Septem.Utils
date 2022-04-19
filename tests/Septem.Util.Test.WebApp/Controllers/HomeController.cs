using Microsoft.AspNetCore.Mvc;
using Septem.Util.Test.WebApp.Models;
using System.Diagnostics;
using Septem.Notifications.Abstractions;

namespace Septem.Util.Test.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotificationTokenService _notificationTokenService;
        private readonly INotificationService _notificationService;

        public HomeController(ILogger<HomeController> logger, INotificationTokenService notificationTokenService, INotificationService notificationService)
        {
            _logger = logger;
            _notificationTokenService = notificationTokenService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var userUid = Guid.Parse("5e4c8c55-5b63-cfa3-4260-c3770027eab6");
            var token =
                "d7Vuc9viSj2Ao432I4vXo3:APA91bHON-PxdIHRtfSDX9XfgxOFFI4GC3rzleU47j3OjMIxyb2J0quHghTg-Mz0_GOIKpyGB3nNTOSJqC4pvkepXyoP9_WT0dMdJX2xq4yUz8QW1Vf1gWi8hHBynlf6kQrg3zCCSPZu";
            await _notificationTokenService.SaveAsync(NotificationToken.Fcm(userUid, token, "ru"));

            await _notificationService.CreateNotificationAsync(new Notification("My data", "my payload").Schedule(DateTime.Now),
                new List<Receiver>
                {
                    Receiver.Fcm().WithTarget(userUid),
                    Receiver.Fcm().WithToken(token),
                    Receiver.Sms().WithToken("994503624553"),
                    Receiver.Email().WithToken("rasim.ismatulin@gmail.com")
                });
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}