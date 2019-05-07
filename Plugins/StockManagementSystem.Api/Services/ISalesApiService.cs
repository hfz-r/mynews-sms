using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface ISalesApiService
    {
        SalesMaster GetSalesById(int id);
        IList<SalesMaster> GetSaless(int limit = 50, int page = 1, int sinceId = 0);
        int GetSalessCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}