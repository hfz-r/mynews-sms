using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Models.Logging;

namespace StockManagementSystem.Factories
{
    public interface IActivityLogModelFactory
    {
        Task<ActivityLogContainerModel> PrepareActivityLogContainerModel(ActivityLogContainerModel activityLogContainerModel);

        Task<ActivityLogListModel> PrepareActivityLogListModel(ActivityLogSearchModel searchModel);

        Task<ActivityLogSearchModel> PrepareActivityLogSearchModel(ActivityLogSearchModel searchModel);

        Task<IList<ActivityLogTypeModel>> PrepareActivityLogTypeModels();
    }
}