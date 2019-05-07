using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface IASNHeaderApiService
    {
        ASNHeaderMaster GetASNHeaderById(int id);
        IList<ASNHeaderMaster> GetASNHeaders(int limit = 50, int page = 1, int sinceId = 0);
        int GetASNHeadersCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}