using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Models.OrderLimits;

namespace StockManagementSystem.Factories
{
    public interface IOrderLimitModelFactory
    {
        Task<OrderLimitModel> PrepareOrderLimitListModel();
        Task<OrderLimitListModel> PrepareOrderLimitListModel(OrderLimitSearchModel searchModel);
        Task<OrderLimitModel> PrepareOrderLimitModel(OrderLimitModel model, OrderLimit orderLimit);
        Task<OrderLimitSearchModel> PrepareOrderLimitSearchModel(OrderLimitSearchModel searchModel);
    }
}