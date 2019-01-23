using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Data;
using StockManagementSystem.Data.Extensions;

namespace StockManagementSystem.Services.Logging
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IDbContext _dbContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        public UserActivityService(
            IRepository<ActivityLog> activityLogRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IDbContext dbContext,
            IStaticCacheManager cacheManager,
            IWebHelper webHelper, 
            IWorkContext workContext)
        {
            _activityLogRepository = activityLogRepository;
            _activityLogTypeRepository = activityLogTypeRepository;
            _dbContext = dbContext;
            _cacheManager = cacheManager;
            _webHelper = webHelper;
            _workContext = workContext;
        }

        /// <summary>
        /// Activity log type for caching
        /// </summary>
        [Serializable]
        public class ActivityLogTypeForCaching
        {
            public int Id { get; set; }

            public string SystemKeyword { get; set; }

            public string Name { get; set; }

            public bool Enabled { get; set; }
        }

        /// <summary>
        /// Gets all activity log types (class for caching)
        /// </summary>
        protected IList<ActivityLogTypeForCaching> GetAllActivityTypesCached()
        {
            return _cacheManager.Get(LoggingDefaults.ActivityTypeAllCacheKey, () =>
            {
                var result = new List<ActivityLogTypeForCaching>();
                var activityLogTypes = GetAllActivityTypesAsync().GetAwaiter().GetResult();
                foreach (var alt in activityLogTypes)
                {
                    var altForCaching = new ActivityLogTypeForCaching
                    {
                        Id = alt.Id,
                        SystemKeyword = alt.SystemKeyword,
                        Name = alt.Name,
                        Enabled = alt.Enabled
                    };
                    result.Add(altForCaching);
                }

                return result;
            });
        }

        #region ActivityLogType

        public async Task InsertActivityTypeAsync(ActivityLogType activityLogType)
        {
            if (activityLogType == null)
                throw new ArgumentNullException(nameof(activityLogType));

            await _activityLogTypeRepository.InsertAsync(activityLogType);
            _cacheManager.RemoveByPattern(LoggingDefaults.ActivityTypePatternCacheKey);
        }

        public async Task InsertActivityTypesAsync(IList<ActivityLogType> activityLogTypes)
        {
            if (activityLogTypes == null)
                throw new ArgumentNullException(nameof(activityLogTypes));

            await _activityLogTypeRepository.InsertAsync(activityLogTypes);
            _cacheManager.RemoveByPattern(LoggingDefaults.ActivityTypePatternCacheKey);
        }

        public async Task UpdateActivityTypeAsync(ActivityLogType activityLogType)
        {
            if (activityLogType == null)
                throw new ArgumentNullException(nameof(activityLogType));

            await _activityLogTypeRepository.UpdateAsync(activityLogType);
            _cacheManager.RemoveByPattern(LoggingDefaults.ActivityTypePatternCacheKey);
        }

        public async Task DeleteActivityTypeAsync(ActivityLogType activityLogType)
        {
            if (activityLogType == null)
                throw new ArgumentNullException(nameof(activityLogType));

            await _activityLogTypeRepository.DeleteAsync(activityLogType);
            _cacheManager.RemoveByPattern(LoggingDefaults.ActivityTypePatternCacheKey);
        }

        public async Task<IList<ActivityLogType>> GetAllActivityTypesAsync()
        {
            var types = await _activityLogTypeRepository.Table
                .OrderBy(type => type.Name)
                .ToListAsync();
            return types;
        }

        public async Task<ActivityLogType> GetActivityTypeByIdAsync(int activityLogTypeId)
        {
            if (activityLogTypeId == 0)
                return null;

            return await _activityLogTypeRepository.GetByIdAsync(activityLogTypeId);
        }

        #endregion

        #region Activity Log

        public async Task<ActivityLog> InsertActivityAsync(string systemKeyword, string comment, BaseEntity entity = null)
        {
            return await InsertActivityAsync(_workContext.CurrentUser, systemKeyword, comment, entity);
        }

        public async Task<ActivityLog> InsertActivityAsync(User user, string systemKeyword, string comment, BaseEntity entity = null)
        {
            if (user == null)
                return null;

            //try to get activity log type by passed system keyword
            var activityLogType = GetAllActivityTypesCached().FirstOrDefault(type => type.SystemKeyword.Equals(systemKeyword));
            if (!activityLogType?.Enabled ?? true)
                return null;

            var logItem = new ActivityLog
            {
                ActivityLogTypeId = activityLogType.Id,
                EntityId = entity?.Id,
                EntityName = entity?.GetUnproxiedEntityType().Name,
                UserId = user.Id,
                Comment = CommonHelper.EnsureMaximumLength(comment ?? string.Empty, 4000),
                CreatedOnUtc = DateTime.UtcNow,
                IpAddress = _webHelper.GetCurrentIpAddress()
            };
            await _activityLogRepository.InsertAsync(logItem);

            return logItem;
        }

        public async Task DeleteActivityAsync(ActivityLog activityLog)
        {
            if (activityLog == null)
                throw new ArgumentNullException(nameof(activityLog));

            await _activityLogRepository.DeleteAsync(activityLog);
        }

        public async Task ClearAllActivitiesAsync()
        {
            var activityLogTableName = _dbContext.GetTableName<ActivityLog>();
            await  _dbContext.ExecuteSqlCommandAsync($"TRUNCATE TABLE [{activityLogTableName}]");
        }

        public IPagedList<ActivityLog> GetAllActivities(
            DateTime? createdOnFrom = null,
            DateTime? createdOnTo = null,
            int? userId = null,
            int? activityLogTypeId = null,
            string ipAddress = null,
            string entityName = null,
            int? entityId = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var query = _activityLogRepository.Table;

            //filter by IP
            if (!string.IsNullOrEmpty(ipAddress))
                query = query.Where(logItem => logItem.IpAddress.Contains(ipAddress));

            //filter by creation date
            if (createdOnFrom.HasValue)
                query = query.Where(logItem => createdOnFrom.Value <= logItem.CreatedOnUtc);
            if (createdOnTo.HasValue)
                query = query.Where(logItem => createdOnTo.Value >= logItem.CreatedOnUtc);

            //filter by log type
            if (activityLogTypeId.HasValue && activityLogTypeId.Value > 0)
                query = query.Where(logItem => activityLogTypeId == logItem.ActivityLogTypeId);

            //filter by user
            if (userId.HasValue && userId.Value > 0)
                query = query.Where(logItem => userId.Value == logItem.UserId);

            //filter by entity
            if (!string.IsNullOrEmpty(entityName))
                query = query.Where(logItem => logItem.EntityName.Equals(entityName));
            if (entityId.HasValue && entityId.Value > 0)
                query = query.Where(logItem => entityId.Value == logItem.EntityId);

            query = query.OrderByDescending(logItem => logItem.CreatedOnUtc).ThenBy(logItem => logItem.Id);

            return new PagedList<ActivityLog>(query, pageIndex, pageSize);
        }

        public async Task<IList<ActivityLog>> GetActivitiesByEntityNameAsync(string entityName = null)
        {
            var activities = _activityLogRepository.Table;

            if (!string.IsNullOrEmpty(entityName))
                activities = activities.Where(logItem => logItem.EntityName.Equals(entityName));

            return await activities.ToListAsync();
        }

        public async Task<ActivityLog> GetActivityByIdAsync(int activityLogId)
        {
            if (activityLogId == 0)
                return null;

            return await _activityLogRepository.GetByIdAsync(activityLogId);
        }

        #endregion
    }
}