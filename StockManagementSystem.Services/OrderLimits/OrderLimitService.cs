using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Services.OrderLimits
{
    public class OrderLimitService : IOrderLimitService
    {
        private readonly IRepository<OrderLimit> _orderLimitRepository;
        private readonly IRepository<OrderLimitStore> _orderLimitStoreRepository;
        private readonly IRepository<Store> _storeRepository;

        public OrderLimitService(
            IRepository<OrderLimit> orderLimitRepository,
            IRepository<OrderLimitStore> orderLimitStoreRepository,
            IRepository<Store> storeRepository)
        {
            _orderLimitRepository = orderLimitRepository;
            _orderLimitStoreRepository = orderLimitStoreRepository;
            _storeRepository = storeRepository;
        }
        
        public Task<IPagedList<OrderLimit>> GetOrderLimitsAsync(
            int[] storeIds = null,
            //int? percentage = 0, //Remove Percentage criteria; Not required - 05032019 
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false)
        {
            var query = _orderLimitRepository.Table;
            var queryStore = _orderLimitStoreRepository.Table;

            if (storeIds != null && storeIds.Length > 0)
            {
                query = query.Where(ol => ol.OrderLimitStores.Any(ols => storeIds.Contains(ols.StoreId)));
            }

            //Remove Percentage criteria; Not required - 05032019 
            //if (percentage > 0)
            //    query = query.Where(u => u.Percentage == percentage);

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<IPagedList<OrderLimit>>(new PagedList<OrderLimit>(query, pageIndex, pageSize,
                getOnlyTotalCount));
        }

        public Task<bool> IsStoreExistAsync(IList<int> storeList)
        {
            bool isExist = false;
            var query = _orderLimitRepository.Table;
            var queryStore = _orderLimitStoreRepository.Table;

            if (storeList != null && storeList.Count > 0)
            {
                query = query.Where(ol => ol.OrderLimitStores.Any(ols => storeList.Contains(ols.StoreId)));
                isExist = query != null && query.Count() > 0 ? true : false;
            }

            return Task.FromResult<bool>(isExist);
        }

        public Task<ICollection<OrderLimitStore>> GetAllOrderLimitsStoreAsync()
        {
            var queryStore = _orderLimitStoreRepository.Table;

            return Task.FromResult<ICollection<OrderLimitStore>>(new List<OrderLimitStore>(queryStore.ToList()));
        }

        public Task<ICollection<OrderLimit>> GetAllOrderLimitsAsync()
        {
            var query = _orderLimitRepository.Table;

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<ICollection<OrderLimit>>(new List<OrderLimit>(query.ToList()));
        }

        public virtual void UpdateOrderLimit(OrderLimit orderLimit)
        {
            if (orderLimit == null)
                throw new ArgumentNullException(nameof(orderLimit));

            _orderLimitRepository.Update(orderLimit);
        }

        public virtual void DeleteOrderLimit(OrderLimit orderLimit)
        {
            if (orderLimit == null)
                throw new ArgumentNullException(nameof(orderLimit));
                
            _orderLimitStoreRepository.Delete(orderLimit.OrderLimitStores);
            _orderLimitRepository.Delete(orderLimit);
        }

        public virtual void DeleteOrderLimitStore(int Id, Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var query = _orderLimitStoreRepository.Table.Where(x => x.OrderLimitId == Id && x.StoreId == store.P_BranchNo);

            _orderLimitStoreRepository.Delete(query);
        }

        public async Task InsertOrderLimit(OrderLimit orderLimit)
        {
            if (orderLimit == null)
                throw new ArgumentNullException(nameof(orderLimit));

            await _orderLimitRepository.InsertAsync(orderLimit);
        }

        #region Identity 

        public async Task<OrderLimit> GetOrderLimitByIdAsync(int orderLimitId)
        {
            if (orderLimitId == 0)
                throw new ArgumentNullException(nameof(orderLimitId));

            var orderLimit = await _orderLimitRepository.Table.FirstOrDefaultAsync(u => u.Id == orderLimitId);
            return orderLimit;
        }
        
        #endregion
    }
}