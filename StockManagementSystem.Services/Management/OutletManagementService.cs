using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Management
{
    public class OutletManagementService : IOutletManagementService
    {
        private readonly IRepository<StoreGrouping> _storeGroupingRepository;
        private readonly IRepository<StoreGroupingStores> _storeGroupingStoresRepository;
        private readonly IRepository<StoreUserAssign> _storeUserAssignRepository;
        private readonly IRepository<StoreUserAssignStores> _storeUserAssignStoresRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<User> _userRepository;

        public OutletManagementService(
            IRepository<StoreGrouping> storeGroupingRepository,
            IRepository<StoreGroupingStores> storeGroupingStoresRepository,
            IRepository<StoreUserAssign> storeUserAssignRepository,
            IRepository<StoreUserAssignStores> storeUserAssignStoresRepository,
            IRepository<Store> storeRepository,
            IRepository<User> userRepository)
        {
            _storeGroupingRepository = storeGroupingRepository;
            _storeGroupingStoresRepository = storeGroupingStoresRepository;
            _storeUserAssignRepository = storeUserAssignRepository;
            _storeUserAssignStoresRepository = storeUserAssignStoresRepository;
            _storeRepository = storeRepository;
            _userRepository = userRepository;
        }

        #region Assign User

        public Task<ICollection<StoreUserAssign>> GetAllAssignUsersAsync()
        {
            var query = _storeUserAssignRepository.Table;

            query = query.OrderByDescending(c => c.CreatedOnUtc);
 
            return Task.FromResult<ICollection<StoreUserAssign>>(new List<StoreUserAssign>(query.ToList()));
        }

        public virtual void UpdateAssignUser(StoreUserAssign storeUserAssign)
        {
            if (storeUserAssign == null)
                throw new ArgumentNullException(nameof(storeUserAssign));

            _storeUserAssignRepository.Update(storeUserAssign);
        }

        public async Task InsertAssignUser(StoreUserAssign storeUserAssign)
        {
            if (storeUserAssign == null)
                throw new ArgumentNullException(nameof(storeUserAssign));

            await _storeUserAssignRepository.InsertAsync(storeUserAssign);
        }

        public virtual void DeleteAssignUser(StoreUserAssign storeUserAssign)
        {
            if (storeUserAssign == null)
                throw new ArgumentNullException(nameof(storeUserAssign));

            _storeUserAssignStoresRepository.Delete(storeUserAssign.StoreUserAssignStore);
            _storeUserAssignRepository.Delete(storeUserAssign);
        }

        public virtual void DeleteAssignUserStore(int Id, Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var query = _storeUserAssignStoresRepository.Table.Where(x => x.StoreUserAssignId == Id && x.StoreUserAssignId == store.P_BranchNo);

            _storeUserAssignStoresRepository.Delete(query);
        }

        public virtual void DeleteAssignUserUsers(int Id, User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var query = _storeUserAssignStoresRepository.Table.Where(x => x.StoreUserAssignId == Id && x.UserId == user.Id);

            _storeUserAssignStoresRepository.Delete(query);
        }

        #endregion

        #region Group Outlet

        public Task<IPagedList<StoreGrouping>> GetGroupOutletsAsync(
            int[] storeIds = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false)
        {
            var query = _storeGroupingRepository.Table;
            var queryStore = _storeGroupingStoresRepository.Table;

            if (storeIds != null && storeIds.Length > 0)
            {
                query = query.Join(_storeRepository.Table, x => x.Id, y => y.P_BranchNo,
                        (x, y) => new { StoreGrouping = x, Store = y })
                    .Where(z => storeIds.Contains(z.Store.P_BranchNo))
                    .Select(z => z.StoreGrouping)
                    .Distinct();
            }

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<IPagedList<StoreGrouping>>(new PagedList<StoreGrouping>(query, pageIndex, pageSize,
                getOnlyTotalCount));
        }

        public Task<ICollection<StoreGrouping>> GetAllGroupOutletsAsync()
        {
            var query = _storeGroupingRepository.Table;

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<ICollection<StoreGrouping>>(new List<StoreGrouping>(query.ToList()));
        }

        public virtual void UpdateGroupOutlet(StoreGrouping storeGrouping)
        {
            if (storeGrouping == null)
                throw new ArgumentNullException(nameof(storeGrouping));

            _storeGroupingRepository.Update(storeGrouping);
        }

        public async Task InsertGroupOutlet(StoreGrouping storeGrouping)
        {
            if (storeGrouping == null)
                throw new ArgumentNullException(nameof(storeGrouping));

            await _storeGroupingRepository.InsertAsync(storeGrouping);
        }

        public virtual void DeleteGroupOutlet(StoreGrouping storeGrouping)
        {
            if (storeGrouping == null)
                throw new ArgumentNullException(nameof(storeGrouping));
            
            _storeGroupingRepository.Delete(storeGrouping);
        }

        public virtual void DeleteStoreGroupingId(int Id, Store store)
        {
            int? value = null;

            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var query = _storeRepository.Table.FirstOrDefault(x => x.StoreGroupingId == Id && x.P_BranchNo == store.P_BranchNo);
            if (query != null)
                query.StoreGroupingId = value ;
            _storeRepository.Update(query);
        }

        #endregion

        #region Identity 

        public async Task<StoreUserAssign> GetAssignUserByIdAsync(int assignUserId)
        {
            if (assignUserId == 0)
                throw new ArgumentNullException(nameof(assignUserId));

            var assignUser = await _storeUserAssignRepository.Table.FirstOrDefaultAsync(u => u.Id == assignUserId);
            return assignUser;
        }

        public async Task<StoreGrouping> GetGroupOutletByIdAsync(int groupOutletId)
        {
            if (groupOutletId == 0)
                throw new ArgumentNullException(nameof(groupOutletId));

            var groupOutlet = await _storeGroupingRepository.Table.FirstOrDefaultAsync(u => u.Id == groupOutletId);
            return groupOutlet;
        }

        #endregion
    }
}
