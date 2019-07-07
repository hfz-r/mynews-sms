using System;
using System.Collections.Generic;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DTOs.ShelfLocation;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Api.Services
{
    public interface IShelfLocationApiService
    {
        IList<ShelfLocation> GetShelfLocation(IList<int> ids = null, int limit = 50, int page = 1, int sinceId = 0, DateTime? createdAtMin = null, DateTime? createdAtMax = null);

        ShelfLocation GetShelfLocationById(int id);

        int GetShelfLocationCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null);

        Search<ShelfLocationDto> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false);
    }
}