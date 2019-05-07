using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class MainCategoryApiService : IMainCategoryApiService
    {
        private readonly IRepository<MainCategoryMaster> _mainCategoryRepository;

        public MainCategoryApiService(IRepository<MainCategoryMaster> mainCategoryRepository)
        {
            _mainCategoryRepository = mainCategoryRepository;
        }

        public IList<MainCategoryMaster> GetMainCategorys(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetMainCategorysQuery(sinceId);

            return new ApiList<MainCategoryMaster>(query, page - 1, limit);
        }

        public MainCategoryMaster GetMainCategoryById(int id)
        {
            if (id <= 0)
                return null;

            var mainCategory = _mainCategoryRepository.Table.FirstOrDefault(i => i.Id == id);

            return mainCategory;
        }

        public int GetMainCategorysCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetMainCategorysQuery(sinceId).Count();
        }

        private IQueryable<MainCategoryMaster> GetMainCategorysQuery(int sinceId = 0)
        {
            var query = _mainCategoryRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}