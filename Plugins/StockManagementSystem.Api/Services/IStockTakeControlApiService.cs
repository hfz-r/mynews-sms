using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface IStockTakeControlApiService
    {
        StockTakeControlMaster GetStockTakeControlById(int id);
        IList<StockTakeControlMaster> GetStockTakeControls(int limit = 50, int page = 1, int sinceId = 0);
        int GetStockTakeControlsCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}