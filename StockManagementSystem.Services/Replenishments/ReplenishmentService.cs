using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Services.Replenishments
{
    public class ReplenishmentService : IReplenishmentService
    {
        private readonly IRepository<Replenishment> _replenishmentRepository;
        private readonly IRepository<ReplenishmentStore> _replenishmentStoreRepository;
        private readonly IRepository<Store> _storeRepository;

        public ReplenishmentService(
            IRepository<Replenishment> replenishmentRepository,
            IRepository<ReplenishmentStore> replenishmentStoreRepository,
            IRepository<Store> storeRepository)
        {
            _replenishmentRepository = replenishmentRepository;
            _replenishmentStoreRepository = replenishmentStoreRepository;
            _storeRepository = storeRepository;
        }

        public Task<IPagedList<Replenishment>> GetReplenishmentsAsync(
            int[] storeIds = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false)
        {
            var query = _replenishmentRepository.Table;
            var queryStore = _replenishmentStoreRepository.Table;

            if (storeIds != null && storeIds.Length > 0)
            {
                query = query.Where(ol => ol.ReplenishmentStores.Any(ols => storeIds.Contains(ols.StoreId)));
            }

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<IPagedList<Replenishment>>(new PagedList<Replenishment>(query, pageIndex, pageSize,
                getOnlyTotalCount));
        }

        public Task<ICollection<Replenishment>> GetAllReplenishmentsAsync()
        {
            var query = _replenishmentRepository.Table;

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<ICollection<Replenishment>>(new List<Replenishment>(query.ToList()));
        }

        public virtual void UpdateReplenishment(Replenishment replenishment)
        {
            if (replenishment == null)
                throw new ArgumentNullException(nameof(replenishment));

            _replenishmentRepository.Update(replenishment);
        }

        public virtual void DeleteReplenishment(Replenishment replenishment)
        {
            if (replenishment == null)
                throw new ArgumentNullException(nameof(replenishment));

            _replenishmentStoreRepository.Delete(replenishment.ReplenishmentStores);
            _replenishmentRepository.Delete(replenishment);
        }

        public virtual void DeleteReplenishmentStore(int Id, Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var query = _replenishmentStoreRepository.Table.Where(x => x.ReplenishmentId == Id && x.StoreId == store.P_BranchNo);

            _replenishmentStoreRepository.Delete(query);
        }

        public async Task InsertReplenishment(Replenishment replenishment)
        {
            if (replenishment == null)
                throw new ArgumentNullException(nameof(replenishment));

            await _replenishmentRepository.InsertAsync(replenishment);
        }

        #region Identity 

        public async Task<Replenishment> GetReplenishmentByIdAsync(int replenishmentId)
        {
            if (replenishmentId == 0)
                throw new ArgumentNullException(nameof(replenishmentId));

            var replenishment = await _replenishmentRepository.Table.FirstOrDefaultAsync(u => u.Id == replenishmentId);
            return replenishment;
        }

        #endregion
    }
}
