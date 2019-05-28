using System;
using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Api.Services
{
    public interface IShelfLocationApiService
    {
        IList<ShelfLocation> GetShelfLocation(IList<int> ids = null, int limit = 50, int page = 1, int sinceId = 0, DateTime? createdAtMin = null, DateTime? createdAtMax = null);

        ShelfLocation GetShelfLocationById(int id);

        int GetShelfLocationCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null);
    }
}