using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class StockTakeControlApiService : IStockTakeControlApiService
    {
        private readonly IRepository<StockTakeControlMaster> _stockTakeControlRepository;

        public StockTakeControlApiService(IRepository<StockTakeControlMaster> stockTakeControlRepository)
        {
            _stockTakeControlRepository = stockTakeControlRepository;
        }

        public IList<StockTakeControlMaster> GetStockTakeControls(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetStockTakeControlsQuery(sinceId);

            return new ApiList<StockTakeControlMaster>(query, page - 1, limit);
        }

        public StockTakeControlMaster GetStockTakeControlById(int id)
        {
            if (id <= 0)
                return null;

            var stockTakeControl = _stockTakeControlRepository.Table.FirstOrDefault(i => i.Id == id);

            return stockTakeControl;
        }

        public int GetStockTakeControlsCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetStockTakeControlsQuery(sinceId).Count();
        }

        private IQueryable<StockTakeControlMaster> GetStockTakeControlsQuery(int sinceId = 0)
        {
            var query = _stockTakeControlRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}