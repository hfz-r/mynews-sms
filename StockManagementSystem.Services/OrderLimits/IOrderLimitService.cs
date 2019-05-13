using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Master;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagementSystem.Services.OrderLimits
{
    public interface IOrderLimitService
    {
        void DeleteOrderLimit(OrderBranchMaster orderLimit);
        Task<ICollection<OrderBranchMaster>> GetAllOrderLimitsAsync();
        Task<OrderBranchMaster> GetOrderLimitByIdAsync(int orderLimitId);
        Task<IPagedList<OrderBranchMaster>> GetOrderLimitsAsync(int[] storeIds = null/*, int? percentage = 0*/, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false); //Remove Percentage criteria; Not required - 05032019
        Task<bool> IsStoreExistAsync(int branchNo);
        Task<ICollection<OrderBranchMaster>> GetAllOrderLimitsStoreAsync();
        Task InsertOrderLimit(OrderBranchMaster orderLimit);
        void UpdateOrderLimit(OrderBranchMaster orderLimit);
    }
}