using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface IOrderBranchApiService
    {
        OrderBranchMaster GetOrderBranchById(int id);
        IList<OrderBranchMaster> GetOrderBranchs(int limit = 50, int page = 1, int sinceId = 0);
        int GetOrderBranchsCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}