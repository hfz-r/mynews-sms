using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Api.Services
{
    public class OrderLimitApiService : IOrderLimitApiService
    {
        private readonly IRepository<OrderLimit> _orderLimitRepository;

        public OrderLimitApiService(IRepository<OrderLimit> orderLimitRepository)
        {
            _orderLimitRepository = orderLimitRepository;
        }

        #region Private methods

        private IQueryable<OrderLimit> GetOrderLimitQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, IList<int> storeIds = null)
        {
            var query = _orderLimitRepository.Table;

            if (createdAtMin != null)
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null)
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);

            if (storeIds != null && storeIds.Count > 0)
            {
                var result = new List<OrderLimit>();

                foreach (var orderLimit in query)
                {
                    var ol = orderLimit;

                    var olStore = orderLimit.OrderLimitStores.Where(x => storeIds.Contains(x.StoreId)).ToList();
                    ol.OrderLimitStores = olStore;

                    result.Add(ol);
                }

                query = result.AsQueryable();
            }

            query = query.OrderBy(ol => ol.Id);

            return query;
        }

        #endregion

        public IList<OrderLimit> GetOrdersLimit(
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            IList<int> storeIds = null)
        {
            var query = GetOrderLimitQuery(createdAtMin, createdAtMax, storeIds);

            if (sinceId > 0)
                query = query.Where(ol => ol.Id > sinceId);

            return new ApiList<OrderLimit>(query, page - 1, limit);
        }

        public int GetOrdersLimitCount()
        {
            return _orderLimitRepository.Table.Count();
        }

        public OrderLimit GetOrderLimitById(int id)
        {
            if (id == 0)
                return null;

            var orderLimit = _orderLimitRepository.Table.FirstOrDefault(r => r.Id == id);

            return orderLimit;
        }
    }
}