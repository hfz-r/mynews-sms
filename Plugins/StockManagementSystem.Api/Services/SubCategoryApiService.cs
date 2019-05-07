using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class SubCategoryApiService : ISubCategoryApiService
    {
        private readonly IRepository<SubCategoryMaster> _subCategoryRepository;

        public SubCategoryApiService(IRepository<SubCategoryMaster> subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public IList<SubCategoryMaster> GetSubCategorys(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetSubCategorysQuery(sinceId);

            return new ApiList<SubCategoryMaster>(query, page - 1, limit);
        }

        public SubCategoryMaster GetSubCategoryById(int id)
        {
            if (id <= 0)
                return null;

            var subCategory = _subCategoryRepository.Table.FirstOrDefault(i => i.Id == id);

            return subCategory;
        }

        public int GetSubCategorysCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetSubCategorysQuery(sinceId).Count();
        }

        private IQueryable<SubCategoryMaster> GetSubCategorysQuery(int sinceId = 0)
        {
            var query = _subCategoryRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}