using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class LogController : BaseController
    {
        private readonly ILogModelFactory _logModelFactory;
        private readonly IUserActivityService _userActivityService;
        private readonly ILogger _logger;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;

        public LogController(
            ILogModelFactory logModelFactory,
            IUserActivityService userActivityService, 
            ILogger logger, 
            IPermissionService permissionService, 
            INotificationService notificationService)
        {
            _logModelFactory = logModelFactory;
            _userActivityService = userActivityService;
            _logger = logger;
            _permissionService = permissionService;
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var model = await _logModelFactory.PrepareLogSearchModel(new LogSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogList(LogSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedKendoGridJson();

            var model = await _logModelFactory.PrepareLogListModel(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("clearall")]
        public async Task<IActionResult> ClearAll()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            _logger.ClearLog();

            await _userActivityService.InsertActivityAsync("DeleteSystemLog", "Deleted system log");

            _notificationService.SuccessNotification("The log has been cleared successfully.");

            return RedirectToAction("List");
        }

        public async Task<IActionResult> View(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = await _logger.GetLogById(id);
            if (log == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _logModelFactory.PrepareLogModel(null, log);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            var log = await _logger.GetLogById(id);
            if (log == null)
                return RedirectToAction("List");

            await _logger.DeleteLog(log);

            await _userActivityService.InsertActivityAsync("DeleteSystemLog", "Deleted system log", log);

            _notificationService.SuccessNotification("The log entry has been deleted successfully.");

            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSystemLog))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var ids = await _logger.GetLogByIds(selectedIds.ToArray());
                await _logger.DeleteLogs(ids);
            }

            await _userActivityService.InsertActivityAsync("DeleteSystemLog", "Deleted system log");

            return Json(new { Result = true });
        }
    }
}