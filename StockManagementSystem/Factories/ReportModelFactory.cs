using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Kendoui.Extensions;

namespace StockManagementSystem.Factories
{
    public class ReportModelFactory : IReportModelFactory
    {
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public ReportModelFactory(
            IUserService userService,
            IUserActivityService userActivityService,
            IDateTimeHelper dateTimeHelper)
        {
            _userService = userService;
            _userActivityService = userActivityService;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<ActivityLogContainerModel> PrepareActivityLogContainerModel(
            ActivityLogContainerModel activityLogContainerModel)
        {
            if (activityLogContainerModel == null)
                throw new ArgumentNullException(nameof(activityLogContainerModel));

            //prepare nested models
            await PrepareSignedInLogSearchModel(activityLogContainerModel.ListSignedIn);
            await PrepareActivityLogSearchModel(activityLogContainerModel.ListLogs);
            activityLogContainerModel.ListTypes = await PrepareActivityLogTypeModels();

            return activityLogContainerModel;
        }

        public async Task<SignedInLogSearchModel> PrepareSignedInLogSearchModel(SignedInLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);
        }

        public async Task<ActivityLogSearchModel> PrepareActivityLogSearchModel(ActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare selectlist
            var availableActivityTypes = await _userActivityService.GetAllActivityTypesAsync();
            foreach (var activityType in availableActivityTypes)
            {
                searchModel.ActivityLogType.Add(new SelectListItem
                {
                    Value = activityType.Id.ToString(),
                    Text = activityType.Name
                });
            }

            //insert "all" at first
            searchModel.ActivityLogType.Insert(0, new SelectListItem {Text = "All", Value = "0"});
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public async Task<IList<ActivityLogTypeModel>> PrepareActivityLogTypeModels()
        {
            var availableActivityTypes = await _userActivityService.GetAllActivityTypesAsync();
            var models = availableActivityTypes.Select(activityType => activityType.ToModel<ActivityLogTypeModel>())
                .ToList();

            return models;
        }

        public async Task<ActivityLogListModel> PrepareActivityLogListModel(ActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var startDateValue = searchModel.CreatedOnFrom == null
                ? null
                : (DateTime?) _dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value,
                    _dateTimeHelper.CurrentTimeZone);
            var endDateValue = searchModel.CreatedOnTo == null
                ? null
                : (DateTime?) _dateTimeHelper
                    .ConvertToUtcTime(searchModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var activityLog = _userActivityService.GetAllActivities(
                createdOnFrom: startDateValue,
                createdOnTo: endDateValue,
                activityLogTypeId: searchModel.ActivityLogTypeId,
                ipAddress: searchModel.IpAddress,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new ActivityLogListModel
            {
                Data = activityLog.Select(logItem =>
                {
                    var logItemModel = logItem.ToModel<ActivityLogModel>();
                    logItemModel.ActivityLogTypeName = logItem.ActivityLogType.Name;
                    logItemModel.UserEmail = logItem.User.Email;
                    logItemModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return logItemModel;
                }),
                Total = activityLog.TotalCount
            };

            // sort
            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            // filter
            if (searchModel.Filter?.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        public async Task<SignedInLogListModel> PrepareSignedInLogListModel(SignedInLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var startLoginDateValue = searchModel.LastLoginFrom == null
                ? null
                : (DateTime?) _dateTimeHelper.ConvertToUtcTime(searchModel.LastLoginFrom.Value,
                    _dateTimeHelper.CurrentTimeZone);
            var endLoginDateValue = searchModel.LastLoginTo == null
                ? null
                : (DateTime?) _dateTimeHelper
                    .ConvertToUtcTime(searchModel.LastLoginTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var users = await _userService.GetUsersAsync(
                lastLoginFrom: startLoginDateValue,
                lastLoginTo: endLoginDateValue,
                ipAddress: searchModel.LastIpAddress,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new SignedInLogListModel
            {
                Data = users.Where(user => user.LastLoginDateUtc != null)
                    .Select(user =>
                    {
                        var signedInModel = user.ToModel<SignedInLogModel>();
                        signedInModel.UserId = user.Id;
                      
                        if (user.LastLoginDateUtc.HasValue)
                            signedInModel.LastLoginDate = _dateTimeHelper.ConvertToUserTime(user.LastLoginDateUtc.Value, 
                                DateTimeKind.Utc);

                        return signedInModel;
                    }),
                Total = users.TotalCount
            };

            // sort
            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            // filter
            if (searchModel.Filter?.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }
    }
}