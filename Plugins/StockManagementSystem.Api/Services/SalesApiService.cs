using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class SalesApiService : ISalesApiService
    {
        private readonly IRepository<SalesMaster> _salesRepository;

        public SalesApiService(IRepository<SalesMaster> salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public IList<SalesMaster> GetSaless(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetSalessQuery(sinceId);

            return new ApiList<SalesMaster>(query, page - 1, limit);
        }

        public SalesMaster GetSalesById(int id)
        {
            if (id <= 0)
                return null;

            var sales = _salesRepository.Table.FirstOrDefault(i => i.Id == id);

            return sales;
        }

        public int GetSalessCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetSalessQuery(sinceId).Count();
        }

        private IQueryable<SalesMaster> GetSalessQuery(int sinceId = 0)
        {
            var query = _salesRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}