using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Web.Kendoui.Extensions;

namespace StockManagementSystem.Factories
{
    public class ActivityLogModelFactory : IActivityLogModelFactory
    {
        private readonly IBaseModelFactory _baseModelFactory;
        private readonly IUserActivityService _userActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public ActivityLogModelFactory(
            IBaseModelFactory baseModelFactory,
            IUserActivityService userActivityService,
            IDateTimeHelper dateTimeHelper)
        {
            _baseModelFactory = baseModelFactory;
            _userActivityService = userActivityService;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<ActivityLogContainerModel> PrepareActivityLogContainerModel(
            ActivityLogContainerModel activityLogContainerModel)
        {
            if (activityLogContainerModel == null)
                throw new ArgumentNullException(nameof(activityLogContainerModel));

            //prepare nested models
            await PrepareActivityLogSearchModel(activityLogContainerModel.ListLogs);
            activityLogContainerModel.ListTypes = await PrepareActivityLogTypeModels();

            return activityLogContainerModel;
        }

        public async Task<ActivityLogSearchModel> PrepareActivityLogSearchModel(ActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare selectlist
            await _baseModelFactory.PrepareActivityLogTypes(searchModel.ActivityLogType);

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
    }
}