using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class ShelfLocationApiService : IShelfLocationApiService
    {
        private readonly IRepository<ShelfLocationMaster> _shelfLocationRepository;

        public ShelfLocationApiService(IRepository<ShelfLocationMaster> shelfLocationRepository)
        {
            _shelfLocationRepository = shelfLocationRepository;
        }

        public IList<ShelfLocationMaster> GetShelfLocations(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetShelfLocationsQuery(sinceId);

            return new ApiList<ShelfLocationMaster>(query, page - 1, limit);
        }

        public ShelfLocationMaster GetShelfLocationById(int id)
        {
            if (id <= 0)
                return null;

            var shelfLocation = _shelfLocationRepository.Table.FirstOrDefault(i => i.Id == id);

            return shelfLocation;
        }

        public int GetShelfLocationsCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetShelfLocationsQuery(sinceId).Count();
        }

        private IQueryable<ShelfLocationMaster> GetShelfLocationsQuery(int sinceId = 0)
        {
            var query = _shelfLocationRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}