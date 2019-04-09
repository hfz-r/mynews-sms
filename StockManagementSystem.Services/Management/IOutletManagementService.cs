using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Services.Management
{
    public interface IOutletManagementService
    {
        void DeleteAssignUser(StoreUserAssign storeUserAssign);
        void DeleteAssignUserStore(int Id, Store store);
        void DeleteAssignUserUsers(int Id, User user);
        void DeleteGroupOutlet(StoreGrouping storeGrouping);
        void DeleteStoreGroupingId(int Id, Store store); //DeleteGroupOutletStore
        Task<ICollection<StoreUserAssign>> GetAllAssignUsersAsync();
        Task<ICollection<StoreGrouping>> GetAllGroupOutletsAsync();
        Task<StoreUserAssign> GetAssignUserByIdAsync(int assignUserId);
        Task<StoreGrouping> GetGroupOutletByIdAsync(int groupOutletId);
        Task<IPagedList<StoreGrouping>> GetGroupOutletsAsync(int[] storeIds = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        Task InsertAssignUser(StoreUserAssign storeUserAssign);
        Task InsertGroupOutlet(StoreGrouping storeGrouping);
        void UpdateAssignUser(StoreUserAssign storeUserAssign);
        void UpdateGroupOutlet(StoreGrouping storeGrouping);
    }
}