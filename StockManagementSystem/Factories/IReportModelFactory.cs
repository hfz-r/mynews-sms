using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Models.Reports;

namespace StockManagementSystem.Factories
{
    public interface IReportModelFactory
    {
        Task<ReportContainerModel> PrepareReportContainerModel(ReportContainerModel reportContainerModel);

        Task<SignedInLogListModel> PrepareSignedInLogListModel(SignedInLogSearchModel searchModel);

        Task<SignedInLogSearchModel> PrepareSignedInLogSearchModel(SignedInLogSearchModel searchModel);

        Task<TransActivitySearchModel> PrepareTransActivitySearchModel(TransActivitySearchModel searchModel);

        Task<IEnumerable<TransActivityModel>> PrepareListTransActivity(TransActivitySearchModel searchModel);
    }
}