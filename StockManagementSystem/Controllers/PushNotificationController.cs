using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StockManagementSystem.Controllers
{
    public class PushNotificationController : Controller
    {
        public IActionResult AddNotification()
        {
            return View("AddNotification");
        }

        public IActionResult Notification()
        {
            return View("Notification");
        }
    }
}