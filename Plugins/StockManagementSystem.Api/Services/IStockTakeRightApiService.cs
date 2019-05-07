using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface IStockTakeRightApiService
    {
        StockTakeRightMaster GetStockTakeRightById(int id);
        IList<StockTakeRightMaster> GetStockTakeRights(int limit = 50, int page = 1, int sinceId = 0);
        int GetStockTakeRightsCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}