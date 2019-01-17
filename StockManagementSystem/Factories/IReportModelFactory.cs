using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Models.Logging;

namespace StockManagementSystem.Factories
{
    public interface IReportModelFactory
    {
        Task<ActivityLogContainerModel> PrepareActivityLogContainerModel(ActivityLogContainerModel activityLogContainerModel);

        Task<ActivityLogListModel> PrepareActivityLogListModel(ActivityLogSearchModel searchModel);

        Task<SignedInLogListModel> PrepareSignedInLogListModel(SignedInLogSearchModel searchModel);

        Task<SignedInLogSearchModel> PrepareSignedInLogSearchModel(SignedInLogSearchModel searchModel);

        Task<ActivityLogSearchModel> PrepareActivityLogSearchModel(ActivityLogSearchModel searchModel);

        Task<IList<ActivityLogTypeModel>> PrepareActivityLogTypeModels();
    }
}