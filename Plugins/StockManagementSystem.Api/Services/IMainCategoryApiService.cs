using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface IMainCategoryApiService
    {
        MainCategoryMaster GetMainCategoryById(int id);
        IList<MainCategoryMaster> GetMainCategorys(int limit = 50, int page = 1, int sinceId = 0);
        int GetMainCategorysCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}