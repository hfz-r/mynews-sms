using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface IBranchApiService
    {
        BranchMaster GetBranchById(int id);
        IList<BranchMaster> GetBranchs(int limit = 50, int page = 1, int sinceId = 0);
        int GetBranchsCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}