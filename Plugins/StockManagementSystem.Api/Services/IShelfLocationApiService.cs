using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface IShelfLocationApiService
    {
        ShelfLocationMaster GetShelfLocationById(int id);
        IList<ShelfLocationMaster> GetShelfLocations(int limit = 50, int page = 1, int sinceId = 0);
        int GetShelfLocationsCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}