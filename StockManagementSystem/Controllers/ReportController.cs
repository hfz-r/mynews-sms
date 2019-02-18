using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Transactions;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Factories;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Models.Reports;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Installation;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Web.Controllers;

namespace StockManagementSystem.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IReportModelFactory _reportModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportController(
            IReportModelFactory reportModelFactory,
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper,
            IHttpContextAccessor httpContextAccessor)
        {
            _reportModelFactory = reportModelFactory;
            _permissionService = permissionService;
            _dateTimeHelper = dateTimeHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var model = await _reportModelFactory.PrepareReportContainerModel(new ReportContainerModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ListSignedIn(SignedInLogSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReports))
                return AccessDeniedKendoGridJson();

            var model = await _reportModelFactory.PrepareSignedInLogListModel(searchModel);

            return Json(model);
        }

        #region Charts

        public async Task<IActionResult> RenderTransActivityPieChart(TransActivitySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var model = await _reportModelFactory.PrepareListTransActivity(searchModel);

            var trans = model
                .GroupBy(x => x.Category).Select(x => new
                {
                    entity = x.Key,
                    value = ((float) x.Count() / model.Count() * 100).ToString("F")
                });

            return Json(trans);
        }

        public async Task<IActionResult> RenderTransActivityBarChart(TransActivitySearchModel searchModel, string period)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReports))
                return AccessDeniedView();

            var model = await _reportModelFactory.PrepareListTransActivity(searchModel);

            var stacked = new List<TransActivityStackedBarModel>();

            var trans = model
                .GroupBy(x => x.Category)
                .Select(x => x.First());

            foreach (var tran in trans.ToList())
            {
                stacked.Add(new TransActivityStackedBarModel
                {
                    stacked = tran.Category,
                    datasets = GetTransActivityDataSet(period, tran.Category, model)
                });
            }

            return Json(stacked);
        }

        public List<DataSet> GetTransActivityDataSet(string period, string category, IEnumerable<TransActivityModel> models)
        {
            var result = new List<DataSet>();
            var query = models.AsQueryable();

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var features = _httpContextAccessor.HttpContext?.Features?.Get<IRequestCultureFeature>();
            var culture = features?.RequestCulture.Culture;

            switch (period)
            {
                case "year":
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var yearToSearch = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (var i = 0; i <= 12; i++)
                    {
                        query = query.Where(item => yearToSearch <= item.CreatedOn);
                        query = query.Where(item => yearToSearch.AddMonths(1) >= item.CreatedOn);

                        result.Add(new DataSet
                        {
                            label = yearToSearch.Date.ToString("Y", culture),
                            data = query.Count(item => item.Category == category).ToString()
                        });

                        yearToSearch = yearToSearch.AddMonths(1);
                    }
                    break;

                case "month":
                    var monthAgoDt = nowDt.AddDays(-30);
                    var monthToSearch = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (var i = 0; i <= 30; i++)
                    {
                        query = query.Where(item => monthToSearch <= item.CreatedOn);
                        query = query.Where(item => monthToSearch.AddDays(1) >= item.CreatedOn);

                        result.Add(new DataSet
                        {
                            label = monthToSearch.Date.ToString("M", culture),
                            data = query.Count(item => item.Category == category).ToString()
                        });

                        monthToSearch = monthToSearch.AddDays(1);
                    }
                    break;

                case "week":
                    var weekAgoDt = nowDt.AddDays(-7);
                    var weekToSearch = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        query = query.Where(item => weekToSearch <= item.CreatedOn);
                        query = query.Where(item => weekToSearch.AddDays(1) >= item.CreatedOn);

                        result.Add(new DataSet
                        {
                            label = weekToSearch.Date.ToString("d dddd", culture),
                            data = query.Count(item => item.Category == category).ToString()
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