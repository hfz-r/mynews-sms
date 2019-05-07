using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class BranchApiService : IBranchApiService
    {
        private readonly IRepository<BranchMaster> _branchRepository;

        public BranchApiService(IRepository<BranchMaster> branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public IList<BranchMaster> GetBranchs(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetBranchsQuery(sinceId);

            return new ApiList<BranchMaster>(query, page - 1, limit);
        }

        public BranchMaster GetBranchById(int id)
        {
            if (id <= 0)
                return null;

            var branch = _branchRepository.Table.FirstOrDefault(i => i.Id == id);

            return branch;
        }

        public int GetBranchsCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetBranchsQuery(sinceId).Count();
        }

        private IQueryable<BranchMaster> GetBranchsQuery(int sinceId = 0)
        {
            var query = _branchRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}