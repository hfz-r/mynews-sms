using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface ISubCategoryApiService
    {
        SubCategoryMaster GetSubCategoryById(int id);
        IList<SubCategoryMaster> GetSubCategorys(int limit = 50, int page = 1, int sinceId = 0);
        int GetSubCategorysCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}