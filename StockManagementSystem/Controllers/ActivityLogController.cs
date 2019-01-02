using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc;

namespace StockManagementSystem.Controllers
{
    public class ActivityLogController : BaseController
    {
        private readonly IActivityLogModelFactory _activityLogModelFactory;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;

        public ActivityLogController(
            IActivityLogModelFactory activityLogModelFactory,
            IUserActivityService userActivityService, 
            IPermissionService permissionService, 
            INotificationService notificationService, 
            ILogger<ActivityLogController> logger)
        {
            _activityLogModelFactory = activityLogModelFactory;
            _userActivityService = userActivityService;
            _permissionService = permissionService;
            _notificationService = notificationService;

            Logger = logger;
        }

        public ILogger Logger { get; }

        public async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var model = await _activityLogModelFactory.PrepareActivityLogContainerModel(new ActivityLogContainerModel());

            return View(model);
        }

        public async Task<IActionResult> ListTypes()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var model = await _activityLogModelFactory.PrepareActivityLogTypeModels();

            return View(model);
        }

        [HttpPost, ActionName("ListTypes")]
        public async Task<IActionResult> SaveTypes(IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            await _userActivityService.InsertActivityAsync("EditActivityLogTypes", "Edited activity log types");

            var selectedActivityTypesIds = form["checkbox_activity_types"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                .Distinct().ToList();

            var activityTypes = await _userActivityService.GetAllActivityTypesAsync();
            foreach (var activityType in activityTypes)
            {
                activityType.Enabled = selectedActivityTypesIds.Contains(activityType.Id);
                await _userActivityService.UpdateActivityTypeAsync(activityType);
            }

            _notificationService.SuccessNotification("The types have been updated successfully.");

            SaveSelectedTabName();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ListLogs()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var model = await _activityLogModelFactory.PrepareActivityLogSearchModel(new ActivityLogSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ListLogs(ActivityLogSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedKendoGridJson();

            var model = await _activityLogModelFactory.PrepareActivityLogListModel(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> AcivityLogDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var logItem = await _userActivityService.GetActivityByIdAsync(id) 
                ?? throw new ArgumentException("No activity log found with the specified id", nameof(id));

            await _userActivityService.DeleteActivityAsync(logItem);
            await _userActivityService.InsertActivityAsync("DeleteActivityLog", "Deleted activity log", logItem);

            return new NullJsonResult();
        }
    }
}