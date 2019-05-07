using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class StockTakeRightApiService : IStockTakeRightApiService
    {
        private readonly IRepository<StockTakeRightMaster> _stockTakeRightRepository;

        public StockTakeRightApiService(IRepository<StockTakeRightMaster> stockTakeRightRepository)
        {
            _stockTakeRightRepository = stockTakeRightRepository;
        }

        public IList<StockTakeRightMaster> GetStockTakeRights(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetStockTakeRightsQuery(sinceId);

            return new ApiList<StockTakeRightMaster>(query, page - 1, limit);
        }

        public StockTakeRightMaster GetStockTakeRightById(int id)
        {
            if (id <= 0)
                return null;

            var stockTakeRight = _stockTakeRightRepository.Table.FirstOrDefault(i => i.Id == id);

            return stockTakeRight;
        }

        public int GetStockTakeRightsCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetStockTakeRightsQuery(sinceId).Count();
        }

        private IQueryable<StockTakeRightMaster> GetStockTakeRightsQuery(int sinceId = 0)
        {
            var query = _stockTakeRightRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}