using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Items;

namespace StockManagementSystem.Api.Services
{
    public class ItemApiService : IItemApiService
    {
        private readonly IRepository<Item> _itemRepository;

        public ItemApiService(IRepository<Item> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public IList<Item> GetItems(int? groupId = null, int? vendorId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetItemsQuery(groupId, vendorId, createdAtMin, createdAtMax, sinceId);

            return new ApiList<Item>(query, page - 1, limit);
        }

        public Item GetItemById(int id)
        {
            if (id <= 0)
                return null;

            var item = _itemRepository.Table.FirstOrDefault(i => i.Id == id);

            return item;
        }

        public int GetItemsCount(int? groupId = null, int? vendorId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetItemsQuery(groupId, vendorId, createdAtMin, createdAtMax, sinceId).Count();
        }

        private IQueryable<Item> GetItemsQuery(int? groupId = null, int? vendorId = null, DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            int sinceId = 0)
        {
            var query = _itemRepository.Table;

            if (groupId != null)
                query = query.Where(i => i.P_GroupId == groupId);

            if (vendorId != null)
                query = query.Where(i => i.VendorId == vendorId);

            if (createdAtMin != null)
                query = query.Where(i => i.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null)
                query = query.Where(i => i.CreatedOnUtc < createdAtMax.Value);

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}