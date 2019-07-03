using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Api.Services
{
    public class ShelfLocationApiService : IShelfLocationApiService
    {
        private readonly IRepository<ShelfLocation> _shelfLocationRepository;

        public ShelfLocationApiService(IRepository<ShelfLocation> shelfLocationRepository)
        {
            _shelfLocationRepository = shelfLocationRepository;
        }

        public IList<ShelfLocation> GetShelfLocation(
            IList<int> ids = null, int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null)
        {
            var query = GetShelfLocationQuery(createdAtMin, createdAtMax, ids);

            if (sinceId > 0)
                query = query.Where(device => device.Id > sinceId);

            return new ApiList<ShelfLocation>(query, page - 1, limit);
        }

        public ShelfLocation GetShelfLocationById(int id)
        {
            if (id <= 0)
                return null;

            var shelfLocation = _shelfLocationRepository.Table.FirstOrDefault(i => i.Id == id);

            return shelfLocation;
        }

        public IEnumerable<ShelfLocation> GetShelfLocationByBranchNo(int branchNo, DateTime? createdAtMin = null, DateTime? createdAtMax = null)
        {
            var shelves = GetShelfLocationQuery(createdAtMin, createdAtMax);

            var query = from t in shelves
                where t.P_BranchNo == branchNo
                select t;

            return query.AsEnumerable();
        }

        public int GetShelfLocationCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null)
        {
            var query = GetShelfLocationQuery(createdAtMin, createdAtMax);

            return query.Count();
        }

        private IQueryable<ShelfLocation> GetShelfLocationQuery(DateTime? createdAtMin = null,
            DateTime? createdAtMax = null, IList<int> ids = null)
        {
            var query = _shelfLocationRepository.Table;

            if (ids != null && ids.Count > 0)
                query = query.Where(d => ids.Contains(d.Id));

            if (createdAtMin != null)
                query = query.Where(d => d.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null)
                query = query.Where(d => d.CreatedOnUtc < createdAtMax.Value);

            query = query.OrderBy(d => d.Id);

            return query;
        }
    }
}