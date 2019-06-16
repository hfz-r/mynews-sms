using System;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Html;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Logging;

namespace StockManagementSystem.Factories
{
    public class LogModelFactory : ILogModelFactory
    {
        private readonly IBaseModelFactory _baseModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILogger _logger;

        public LogModelFactory(IBaseModelFactory baseModelFactory, IDateTimeHelper dateTimeHelper, ILogger logger)
        {
            _baseModelFactory = baseModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _logger = logger;
        }

        public async Task<LogSearchModel> PrepareLogSearchModel(LogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            await _baseModelFactory.PrepareLogLevels(searchModel.AvailableLogLevels);

            searchModel.SetGridPageSize();

            return searchModel;
        }

        public async Task<LogListModel> PrepareLogListModel(LogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter log
            var createdOnFromValue = searchModel.CreatedOnFrom.HasValue
                ? (DateTime?) _dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value,
                    _dateTimeHelper.CurrentTimeZone)
                : null;
            var createdToFromValue = searchModel.CreatedOnTo.HasValue
                ? (DateTime?) _dateTimeHelper
                    .ConvertToUtcTime(searchModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1)
                : null;
            var logLevel = searchModel.LogLevelId > 0 ? (LogLevel?)searchModel.LogLevelId : null;

            var logItems = await _logger.GetAllLogs(message: searchModel.Message,
                fromUtc: createdOnFromValue,
                toUtc: createdToFromValue,
                logLevel: logLevel,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new LogListModel
            {
                Data = logItems.Select(logItem =>
                {
                    var logModel = logItem.ToModel<LogModel>();

                    logModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);
                    logModel.LogLevel = CommonHelper.ConvertEnum(logItem.LogLevel.ToString());
                    logModel.ShortMessage = HtmlHelper.FormatText(logItem.ShortMessage, false, true, false, false, false, false);
                    logModel.FullMessage = string.Empty;
                    logModel.UserEmail = logItem.User?.Email ?? string.Empty;

                    return logModel;
                }),
                Total = logItems.TotalCount
            };

            return model;
        }

        public Task<LogModel> PrepareLogModel(LogModel model, Log log, bool excludeProperties = false)
        {
            if (log != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = log.ToModel<LogModel>();

                    model.LogLevel = CommonHelper.ConvertEnum(log.LogLevel.ToString());
                    model.ShortMessage = HtmlHelper.FormatText(log.ShortMessage, false, true, false, false, false, false);
                    model.FullMessage = HtmlHelper.FormatText(log.FullMessage, false, true, false, false, false, false);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc);
                    model.FullMessage = string.Empty;
                    model.UserEmail = log.User?.Email ?? string.Empty;
                }
            }

            return Task.FromResult(model);
        }
    }
}