using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Logging
{
    public interface IUserActivityService
    {
        Task DeleteActivityAsync(ActivityLog activityLog);

        Task DeleteActivityTypeAsync(ActivityLogType activityLogType);

        void ClearAllActivities();

        Task<ActivityLog> GetActivityByIdAsync(int activityLogId);

        Task<ActivityLogType> GetActivityTypeByIdAsync(int activityLogTypeId);

        Task<IList<ActivityLog>> GetActivitiesByEntityNameAsync(string entityName = null);

        IPagedList<ActivityLog> GetAllActivities(DateTime? createdOnFrom = null, DateTime? createdOnTo = null,
            int? userId = null, int? activityLogTypeId = null, string ipAddress = null, string entityName = null,
            int? entityId = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IList<ActivityLogType>> GetAllActivityTypesAsync();

        Task<ActivityLog> InsertActivityAsync(string systemKeyword, string comment, BaseEntity entity = null);

        Task<ActivityLog> InsertActivityAsync(User user, string systemKeyword, string comment, BaseEntity entity = null);

        Task InsertActivityTypeAsync(ActivityLogType activityLogType);

        Task InsertActivityTypesAsync(IList<ActivityLogType> activityLogTypes);

        Task UpdateActivityTypeAsync(ActivityLogType activityLogType);
    }
}