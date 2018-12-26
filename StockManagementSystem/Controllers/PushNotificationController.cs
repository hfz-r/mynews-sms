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

        public PushNotificationController(
            IRepository<NotificationCategory> notificationCategory,
            IRepository<PushNotifications> pushNotifications)
        {
            this._notificationCategoryRepository = notificationCategory;
            this._pushNotificationsRepository = pushNotifications;
        }

        //
        // GET: /PushNotification/Notification
        [HttpGet]
        public IActionResult Notification()
        {
            NotificationViewModel model = new NotificationViewModel();
            model.PushNotificationsList = _pushNotificationsRepository.Table.ToList();

            return View(model);
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
                    NotificationCategoryId = model.NotificationCategoryId

                };
                _pushNotificationsRepository.Insert(pushNotifications);
            }
            return RedirectToAction(nameof(PushNotificationController.Notification), "PushNotification");
        }

        //
        // GET: /PushNotification/EditNotification
        [HttpGet]
        [Route("[Controller]/PushNotification/EditNotification/{Id}")]
        public IActionResult EditNotification(int? Id)
        {
            EditNotificationViewModel model = new EditNotificationViewModel();
            var pushNotifications = _pushNotificationsRepository.GetById(Id);
            
            var category = _notificationCategoryRepository.Table.ToList();

            if (pushNotifications != null)
            {
                model.Title = pushNotifications.Title;
                model.Desc = pushNotifications.Desc;
                model.NotificationCategoryId = pushNotifications.NotificationCategoryId;
                model.NotificationCategories = category;

                if (pushNotifications.IsShift == "Today")
                {
                    model.IsShift = Repeat.Today;
                }
                else if (pushNotifications.IsShift == "Weekly")
                {
                    model.IsShift = Repeat.Weekly;
                }
                else if (pushNotifications.IsShift == "Monthly")
                {
                    model.IsShift = Repeat.Monthly;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Notification list not found.");
            }
            return View(model);
        }

        //
        // POST: /PushNotification/EditNotification
        [HttpPost]
        [Route("[Controller]/PushNotification/EditNotification/{Id}")]
        public IActionResult EditNotification(EditNotificationViewModel model, int? Id)
        {
            if (ModelState.IsValid)
            {
                var pushNotifications = _pushNotificationsRepository.GetById(Id);

                if (Id == pushNotifications.Id)
                {
                    pushNotifications.Title = model.Title;
                    pushNotifications.Desc = model.Desc;
                    pushNotifications.IsShift = model.IsShift.ToString();
                    pushNotifications.NotificationCategoryId = model.NotificationCategoryId;
                    //pushNotifications.BranchId = model.
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Update notification failed!.");
                }
                _pushNotificationsRepository.Update(pushNotifications);
            }
                return RedirectToAction(nameof(PushNotificationController.Notification), "PushNotification");
        }

        //
        // POST: Delete
        [HttpPost]
        public IActionResult DeleteNotification(int Id)
        {
            if (ModelState.IsValid)
            {
                var pushNotifications = _pushNotificationsRepository.GetById(Id);
                if (pushNotifications != null && Id == pushNotifications.Id)
                {
                    _pushNotificationsRepository.Delete(pushNotifications);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Delete notification failed!.");
                }
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