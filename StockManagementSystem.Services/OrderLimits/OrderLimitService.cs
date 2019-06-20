using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Services.OrderLimits
{
    public class OrderLimitService : IOrderLimitService
    {
        private readonly IRepository<OrderBranchMaster> _orderLimitRepository;
        //private readonly IRepository<OrderLimitStore> _orderLimitStoreRepository;
        private readonly IRepository<Store> _storeRepository;

        public OrderLimitService(
            IRepository<OrderBranchMaster> orderLimitRepository,
            //IRepository<OrderLimitStore> orderLimitStoreRepository,
            IRepository<Store> storeRepository)
        {
            _orderLimitRepository = orderLimitRepository;
            //_orderLimitStoreRepository = orderLimitStoreRepository;
            _storeRepository = storeRepository;
        }
        
        public Task<IPagedList<OrderBranchMaster>> GetOrderLimitsAsync(
            int[] storeIds = null,
            //int? percentage = 0, //Remove Percentage criteria; Not required - 05032019 
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false)
        {
            var query = _orderLimitRepository.Table;
            //var queryStore = _orderLimitStoreRepository.Table;

            //if (storeIds != null && storeIds.Length > 0)
            //{
            //    query = query.Where(ol => ol.OrderLimitStores.Any(ols => storeIds.Contains(ols.StoreId)));
            //}

            //Remove Percentage criteria; Not required - 05032019 
            //if (percentage > 0)
            //    query = query.Where(u => u.Percentage == percentage);

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<IPagedList<OrderBranchMaster>>(new PagedList<OrderBranchMaster>(query, pageIndex, pageSize,
                getOnlyTotalCount));
        }

        //public Task<bool> IsStoreExistAsync(int branchNo)
        //{
        //    bool isExist = false;
        //    var query = _orderLimitRepository.Table;

        //    isExist = query.Any(x => x.P_BranchNo == branchNo);

        //    return Task.FromResult<bool>(isExist);
        //}

        public Task<ICollection<OrderBranchMaster>> GetAllOrderLimitsStoreAsync()
        {
            var queryStore = _orderLimitRepository.Table;

            return Task.FromResult<ICollection<OrderBranchMaster>>(new List<OrderBranchMaster>(queryStore.ToList()));
        }

        public Task<ICollection<OrderBranchMaster>> GetAllOrderLimitsAsync()
        {
            var query = _orderLimitRepository.Table;

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<ICollection<OrderBranchMaster>>(new List<OrderBranchMaster>(query.ToList()));
        }

        public virtual void UpdateOrderLimit(OrderBranchMaster orderLimit)
        {
            if (orderLimit == null)
                throw new ArgumentNullException(nameof(orderLimit));

            _orderLimitRepository.Update(orderLimit);
        }

        public virtual void DeleteOrderLimit(OrderBranchMaster orderLimit)
        {
            if (orderLimit == null)
                throw new ArgumentNullException(nameof(orderLimit));
                
            //_orderLimitStoreRepository.Delete(orderLimit.OrderLimitStores);
            _orderLimitRepository.Delete(orderLimit);
        }

        //public virtual void DeleteOrderLimitStore(int Id, Store store)
        //{
        //    if (store == null)
        //        throw new ArgumentNullException(nameof(store));

        //    var query = _orderLimitStoreRepository.Table.Where(x => x.OrderLimitId == Id && x.StoreId == store.P_BranchNo);

        //    _orderLimitStoreRepository.Delete(query);
        //}

        public async Task InsertOrderLimit(OrderBranchMaster orderLimit)
        {
            if (orderLimit == null)
                throw new ArgumentNullException(nameof(orderLimit));

            await _orderLimitRepository.InsertAsync(orderLimit);
        }

        #region Identity 

        public async Task<OrderBranchMaster> GetOrderLimitByIdAsync(int orderLimitId)
        {
            if (orderLimitId == 0)
                throw new ArgumentNullException(nameof(orderLimitId));

            var orderLimit = await _orderLimitRepository.Table.FirstOrDefaultAsync(u => u.Id == orderLimitId);
            return orderLimit;
        }
        
        #endregion
    }
}