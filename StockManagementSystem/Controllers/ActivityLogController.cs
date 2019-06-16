using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Models.Reports;
using StockManagementSystem.Services.Helpers;
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
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ActivityLogController(
            IActivityLogModelFactory activityLogModelFactory,
            IUserActivityService userActivityService, 
            IPermissionService permissionService,
            INotificationService notificationService,
            IDateTimeHelper dateTimeHelper,
            IHttpContextAccessor httpContextAccessor)
        {
            _activityLogModelFactory = activityLogModelFactory;
            _userActivityService = userActivityService;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _dateTimeHelper = dateTimeHelper;
            _httpContextAccessor = httpContextAccessor;
        }

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
                return AccessDeniedView();

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

        [HttpPost]
        public async Task<IActionResult> EntityGroupList()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var entities = (await _userActivityService.GetActivitiesByEntityNameAsync())
                .Where(u => !string.IsNullOrWhiteSpace(u.EntityName))
                .GroupBy(u => u.EntityName)
                .Select(u => new ActivityLogModel
                {
                    Id = u.First().Id,
                    EntityName = u.First().EntityName
                })
                .OrderBy(u => u.EntityName);

            return Json(entities);
        }

        public async Task<IActionResult> ClearAll()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            _userActivityService.ClearAllActivities();

            await _userActivityService.InsertActivityAsync("DeleteActivityLog", "Deleted activity log");

            return RedirectToAction("Index");
        }

        #region Charts

        public async Task<IActionResult> GetTransActivityPieData()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var total = (await _userActivityService.GetActivitiesByEntityNameAsync())
                .Where(u => !string.IsNullOrEmpty(u.EntityName)).ToList();

            var activities = _userActivityService.GetAllActivities()
                .Where(e => !string.IsNullOrWhiteSpace(e.EntityName))
                .GroupBy(activity => activity.EntityName)
                .Select(e => new
                {
                    entity = e.Key,
                    value = ((float)e.Count() / total.Count * 100).ToString("F")
                });

            return Json(activities);
        }

        public async Task<IActionResult> GetTransActivityStackedBarData(string period)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var stacked = new List<TransActivityStackedBarModel>();

            var searchEntities = (await _userActivityService.GetActivitiesByEntityNameAsync())
                .Where(u => !string.IsNullOrEmpty(u.EntityName))
                .GroupBy(u => u.EntityName)
                .Select(u => u.First());

            foreach (var entity in searchEntities.ToList())
            {
                stacked.Add(new TransActivityStackedBarModel
                {
                    stacked = entity.EntityName,
                    datasets = GetDataSet(period, entity.EntityName)
                });
            }

            return Json(stacked);
        }

        public List<DataSet> GetDataSet(string period, string name)
        {
            var result = new List<DataSet>();

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var timeZone = _dateTimeHelper.CurrentTimeZone;
            var features = _httpContextAccessor.HttpContext?.Features?.Get<IRequestCultureFeature>();
            var culture = features?.RequestCulture.Culture;

            switch (period)
            {
                case "year":
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var yearToSearch = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (int i = 0; i <= 12; i++)
                    {
                        result.Add(new DataSet
                        {
                            label = yearToSearch.Date.ToString("Y", culture),
                            data = _userActivityService.GetAllActivities(
                                    createdOnFrom: _dateTimeHelper.ConvertToUtcTime(yearToSearch, timeZone),
                                    createdOnTo: _dateTimeHelper.ConvertToUtcTime(yearToSearch.AddMonths(1), timeZone),
                                    entityName: name)
                                .TotalCount.ToString()
                        });

                        yearToSearch = yearToSearch.AddMonths(1);
                    }
                    break;

                case "month":
                    var monthAgoDt = nowDt.AddDays(-30);
                    var monthToSearch = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (int i = 0; i <= 30; i++)
                    {
                        result.Add(new DataSet
                        {
                            label = monthToSearch.Date.ToString("M", culture),
                            data = _userActivityService.GetAllActivities(
                                    createdOnFrom: _dateTimeHelper.ConvertToUtcTime(monthToSearch, timeZone),
                                    createdOnTo: _dateTimeHelper.ConvertToUtcTime(monthToSearch.AddDays(1), timeZone),
                                    entityName: name)
                                .TotalCount.ToString()
                        });

                        monthToSearch = monthToSearch.AddDays(1);
                    }
                    break;

                case "week":
                    var weekAgoDt = nowDt.AddDays(-7);
                    var weekToSearch = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        result.Add(new DataSet
                        {
                            label = weekToSearch.Date.ToString("d dddd", culture),
                            data = _userActivityService.GetAllActivities(
                                    createdOnFrom: _dateTimeHelper.ConvertToUtcTime(weekToSearch, timeZone),
                                    createdOnTo: _dateTimeHelper.ConvertToUtcTime(weekToSearch.AddDays(1), timeZone),
                                    entityName: name)
                                .TotalCount.ToString()
                        });

                        weekToSearch = weekToSearch.AddDays(1);
                    }
                    break;
            }

            return result;
        }

        #endregion
    }
}