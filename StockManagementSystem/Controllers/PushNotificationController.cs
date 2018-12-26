using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Domain.PushNotification;
using StockManagementSystem.Models.PushNotification;

namespace StockManagementSystem.Controllers
{
    public class PushNotificationController : Controller
    {
        private readonly IRepository<NotificationCategory> _notificationCategoryRepository;
        private readonly IRepository<PushNotifications> _pushNotificationsRepository;
        private readonly IRepository<PushNotificationCategory> _pushNotificationCategoryRepository;

        public PushNotificationController(
            IRepository<NotificationCategory> notificationCategory,
            IRepository<PushNotifications> pushNotifications,
            IRepository<PushNotificationCategory> pushNotificationCategoryRepository)
        {
            this._notificationCategoryRepository = notificationCategory;
            this._pushNotificationsRepository = pushNotifications;
            this._pushNotificationCategoryRepository = pushNotificationCategoryRepository;
        }

        //
        // GET: /PushNotification/Notification
        public IActionResult Notification()
        {
            return View("Notification");
        }

        //
        // GET: /PushNotification/AddNotification
        [HttpGet]
        //[Route("[Controller]/PushNotification/AddNotification")]
        public IActionResult AddNotification()
        {
            AddNotificationViewModel model = new AddNotificationViewModel();
            var notificationCategories = _notificationCategoryRepository.Table.ToList();
            {
                model.NotificationCategories = notificationCategories;
            }
            return View(model);
        }

        //
        // POST: /PushNotification/AddNotification
        [HttpPost]
        [Route("[Controller]/PushNotification/AddNotification")]
        public IActionResult AddNotification(AddNotificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pushNotifications = new PushNotifications()
                {
                    Title = model.Title,
                    Desc = model.Desc,
                    CreatedOn = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    IsShift = model.IsShift.ToString(),
                };
                _pushNotificationsRepository.Insert(pushNotifications);

                //selected dropdownlist
                PushNotificationCategory pushNotificationCategory = new PushNotificationCategory()
                {
                    NotificationCategoryId = model.NotificationCategoryId,
                    PushNotificationsId = pushNotifications.Id,
                };
                _pushNotificationCategoryRepository.Insert(pushNotificationCategory);

            }
            return RedirectToAction(nameof(PushNotificationController.Notification), "PushNotification");
        }

        //
        // GET: /PushNotification/EditNotification
        [HttpGet]
        public IActionResult EditNotification()
        {
            return View();
        }

        //
        // POST: Delete
        [HttpPost]
        public IActionResult DeleteNotification(string id)
        { 
            var result = _pushNotificationsRepository.GetById(id);
            if (result != null)
            {
                _pushNotificationsRepository.Delete(result);
            }
            return RedirectToAction(nameof(PushNotificationController.Notification), "PushNotification");  
        }

        #region enum
        public enum Repeat
        {
            Today,
            Weekly,
            Monthly
        }
        #endregion
    }
}