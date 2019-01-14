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
using StockManagementSystem.Core.Domain.Stores;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Controllers
{
    public class PushNotificationController : Controller
    {
        private readonly IRepository<NotificationCategory> _notificationCategoryRepository;
        private readonly IRepository<PushNotifications> _pushNotificationsRepository;
        private readonly IRepository<PushNotificationStore> _pushNotificationStoreRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly ILogger _logger;

        #region Constructor

        public PushNotificationController(
            IRepository<NotificationCategory> notificationCategory,
            IRepository<PushNotifications> pushNotifications,
            IRepository<PushNotificationStore> pushNotificationStore,
            IRepository<Store> store,
            ILoggerFactory loggerFactory)
        {
            this._notificationCategoryRepository = notificationCategory;
            this._pushNotificationsRepository = pushNotifications;
            this._pushNotificationStoreRepository = pushNotificationStore;
            this._storeRepository = store;
            _logger = loggerFactory.CreateLogger<PushNotificationController>();
        }

        #endregion

        #region Destructor

        ~PushNotificationController()
        {
            Dispose(false);
        }

        #endregion

        #region PushNotification

        //
        // GET: /PushNotification/Notification
        [HttpGet]
        public IActionResult Notification()
        {
            NotificationViewModel model = new NotificationViewModel();
            if (ModelState.IsValid)
            {
                model.PushNotificationStore = _pushNotificationStoreRepository.Table.ToList();
            }
            return View(model);
        }

        //
        // GET: /PushNotification/AddNotification
        [HttpGet]
        public IActionResult AddNotification()
        {
            AddNotificationViewModel model = new AddNotificationViewModel();
            var notificationCategory = _notificationCategoryRepository.Table.ToList();
            var store = _storeRepository.Table.ToList();
            {
                model.NotificationCategories = notificationCategory;
                model.Stores = store;
            }
            return View(model);
        }

        //
        // POST: /PushNotification/AddNotification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNotification(AddNotificationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var pushNotifications = new PushNotifications
                    {
                        Title = model.Title,
                        Desc = model.Desc,
                        CreatedOnUtc = DateTime.UtcNow,
                        CreatedBy = User.Identity.Name,
                        IsShift = model.IsShift.ToString(),
                        NotificationCategoryId = model.NotificationCategoryId
                    };
                    _pushNotificationsRepository.Insert(pushNotifications);

                    var pushNotificationStore = new PushNotificationStore
                    {
                        PushNotificationId = pushNotifications.Id,
                        StoreId = model.StoreId,
                        CreatedBy = User.Identity.Name,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    _pushNotificationStoreRepository.Insert(pushNotificationStore);
                    _logger.LogInformation(3, "PushNotificationStore(" + pushNotificationStore.Id + ") added successfully.");
                    return RedirectToAction(nameof(PushNotificationController.Notification), "PushNotification");
                }
                return View("AddNotification", model);
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
            }
            return View("AddNotification", model);
        }

        //
        // GET: /PushNotification/EditNotification
        [HttpGet]
        //[Route("[Controller]/PushNotification/EditNotification/{Id}")]
        public IActionResult EditNotification(int? Id)
        {
            return View("EditNotification", GetPushNotification(Id));
        }

        //
        // POST: /PushNotification/EditNotification
        [HttpPost]
       // [Route("[Controller]/PushNotification/EditNotification/{Id}")]
        public IActionResult EditNotification(EditNotificationViewModel model, int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var pushNotificationStore = _pushNotificationStoreRepository.Table.FirstOrDefault(x => x.Id == model.Id);

                    pushNotificationStore.PushNotifications.Title = model.Title;
                    pushNotificationStore.PushNotifications.Desc = model.Desc;
                    pushNotificationStore.PushNotifications.IsShift = model.IsShift.ToString();
                    pushNotificationStore.PushNotifications.NotificationCategoryId = model.NotificationCategoryId;
                    pushNotificationStore.PushNotifications.ModifiedBy = User.Identity.Name;
                    pushNotificationStore.PushNotifications.ModifiedOnUtc = DateTime.UtcNow;
                    pushNotificationStore.StoreId = model.StoreId;
                    pushNotificationStore.ModifiedBy = User.Identity.Name;
                    pushNotificationStore.ModifiedOnUtc = DateTime.UtcNow;

                    _pushNotificationStoreRepository.Update(pushNotificationStore);
                    _logger.LogInformation(3, "PushNotificationStore(" + pushNotificationStore.Id + ") edited successfully.");
                    return RedirectToAction(nameof(PushNotificationController.Notification), "PushNotification");
                }
                // If we got this far, something failed, redisplay form
                return View("EditNotification", model);
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
            }
            return RedirectToAction(nameof(PushNotificationController.Notification), "PushNotification");
        }

        //
        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteNotification(int? Id)
        {
            if (Id == null)
            {
                return BadRequest();
            }

            PushNotificationStore pushNotificationStore = _pushNotificationStoreRepository.GetById(Id);
            PushNotifications pushNotifications = pushNotificationStore.PushNotifications;
            if (pushNotificationStore.PushNotificationId == pushNotifications.Id)
            {
                _pushNotificationStoreRepository.Delete(pushNotificationStore);
                _pushNotificationsRepository.Delete(pushNotifications);
            }

            if (pushNotificationStore == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(PushNotificationController.Notification), "PushNotification");
        }

        #endregion

        #region enum
        public enum Repeat
        {
            Today,
            Weekly,
            Monthly
        }
        #endregion

        #region Private Method
        private void AddErrors(string result)
        {
            ModelState.AddModelError(string.Empty, result);
        }

        private NotificationViewModel GetAllPushNotification()
        {
            NotificationViewModel model = new NotificationViewModel
            {
                PushNotificationStore = _pushNotificationStoreRepository.Table.ToList()
            };

            return model;
        }

        private AddNotificationViewModel GetAllNotificationCategory()
        {
            AddNotificationViewModel model = new AddNotificationViewModel
            {
                NotificationCategories = _notificationCategoryRepository.Table.ToList()
            };

            return model;
        }     

        private AddNotificationViewModel GetAllStore()
        {
            AddNotificationViewModel model = new AddNotificationViewModel
            {
                Stores = _storeRepository.Table.OrderBy(x => x.P_Name).ToList()
            };

            return model;
        }

        private EditNotificationViewModel GetPushNotification(int? id)
        {
            PushNotificationStore pushNotificationStore = _pushNotificationStoreRepository.GetById(id);
            PushNotifications pushNotifications = pushNotificationStore.PushNotifications;

            EditNotificationViewModel model = new EditNotificationViewModel
            {
                Id = id,
                Title = pushNotifications.Title,
                Desc = pushNotifications.Desc,
                NotificationCategoryId = pushNotifications.NotificationCategoryId,
                NotificationCategories = _notificationCategoryRepository.Table.OrderBy(x => x.Name).ToList(),
                StoreId = pushNotificationStore.StoreId,
                Stores = _storeRepository.Table.OrderBy(x => x.P_Name).ToList()
            };

            if (pushNotificationStore.PushNotifications.IsShift == "Today")
            {
                model.IsShift = Repeat.Today;
            }
            else if (pushNotificationStore.PushNotifications.IsShift == "Weekly")
            {
                model.IsShift = Repeat.Weekly;
            }
            else if (pushNotificationStore.PushNotifications.IsShift == "Monthly")
            {
                model.IsShift = Repeat.Monthly;
            }

            return model;
        }
        #endregion
    }
}