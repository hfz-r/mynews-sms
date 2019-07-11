using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Api.DTOs.ShelfLocation;
using StockManagementSystem.Api.Extensions;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Settings;
using static StockManagementSystem.Api.Extensions.ApiServiceExtension;

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

        public Search<ShelfLocationDto> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false)
        {
            var query = _shelfLocationRepository.Table;

            var searchParams = EnsureSearchQueryIsValid(queryParams, ResolveSearchQuery);
            if (searchParams != null)
            {
                query = query.HandleSearchParams(searchParams);
            }

            query = query.GetQuery(sortColumn, @descending);

            var _ = new SearchWrapper<ShelfLocationDto, ShelfLocation>();
            return count
                ? _.ToCount(query)
                : _.ToList(query, page, limit,
                    list => list.Select(entity => entity.ToDto()).ToList() as IList<ShelfLocationDto>);
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