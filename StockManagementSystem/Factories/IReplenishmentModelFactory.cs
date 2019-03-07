using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Models.Replenishments;

namespace StockManagementSystem.Factories
{
    public interface IReplenishmentModelFactory
    {
        Task<ReplenishmentModel> PrepareReplenishmentListModel();
        Task<ReplenishmentListModel> PrepareReplenishmentListModel(ReplenishmentSearchModel searchModel);
        Task<ReplenishmentModel> PrepareReplenishmentModel(ReplenishmentModel model, Replenishment replenishment);
        Task<ReplenishmentSearchModel> PrepareReplenishmentSearchModel(ReplenishmentSearchModel searchModel);
    }
}