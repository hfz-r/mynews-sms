using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Management
{
    public interface IOutletManagementService
    {
        void DeleteAssignUser(StoreUserAssign storeUserAssign);
        void DeleteAssignUserStore(int Id, Store store);
        void DeleteAssignUserUsers(int Id, User user);
        void DeleteGroupOutlet(StoreGrouping storeGrouping);
        void DeleteGroupOutletStore(int Id, Store store);
        Task<ICollection<StoreUserAssign>> GetAllAssignUsersAsync();
        Task<ICollection<StoreGrouping>> GetAllGroupOutletsAsync();
        Task<StoreUserAssign> GetAssignUserByIdAsync(int assignUserId);
        Task<ICollection<StoreUserAssignStores>> GetAssignUserByUserIdAsync(int userID);
        Task<IPagedList<StoreUserAssign>> GetAssignUsersAsync(int[] storeIds = null, int[] userIds = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        Task<ICollection<StoreUserAssign>> GetAssignUsersByStoreIdAsync(int storeID);
        Task<StoreGrouping> GetGroupOutletByIdAsync(int groupOutletId);
        Task<IPagedList<StoreGrouping>> GetGroupOutletsAsync(int[] storeIds = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        Task InsertAssignUser(StoreUserAssign storeUserAssign);
        Task InsertGroupOutlet(StoreGrouping storeGrouping);
        void UpdateAssignUser(StoreUserAssign storeUserAssign);
        void UpdateGroupOutlet(StoreGrouping storeGrouping);
    }
}