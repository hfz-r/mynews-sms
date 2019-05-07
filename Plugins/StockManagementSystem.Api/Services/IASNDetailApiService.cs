using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface IASNDetailApiService
    {
        ASNDetailMaster GetASNDetailById(int id);
        IList<ASNDetailMaster> GetASNDetails(int limit = 50, int page = 1, int sinceId = 0);
        int GetASNDetailsCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}