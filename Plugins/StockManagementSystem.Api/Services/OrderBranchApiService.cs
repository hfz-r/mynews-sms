using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class OrderBranchApiService : IOrderBranchApiService
    {
        private readonly IRepository<OrderBranchMaster> _orderBranchRepository;

        public OrderBranchApiService(IRepository<OrderBranchMaster> orderBranchRepository)
        {
            _orderBranchRepository = orderBranchRepository;
        }

        public IList<OrderBranchMaster> GetOrderBranchs(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetOrderBranchsQuery(sinceId);

            return new ApiList<OrderBranchMaster>(query, page - 1, limit);
        }

        public OrderBranchMaster GetOrderBranchById(int id)
        {
            if (id <= 0)
                return null;

            var orderBranch = _orderBranchRepository.Table.FirstOrDefault(i => i.Id == id);

            return orderBranch;
        }

        public int GetOrderBranchsCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetOrderBranchsQuery(sinceId).Count();
        }

        private IQueryable<OrderBranchMaster> GetOrderBranchsQuery(int sinceId = 0)
        {
            var query = _orderBranchRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}