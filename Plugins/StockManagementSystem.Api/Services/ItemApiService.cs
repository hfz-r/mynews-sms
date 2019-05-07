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

        public IList<Item> GetItems(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetItemsQuery(sinceId);

            return new ApiList<Item>(query, page - 1, limit);
        }

        public Item GetItemById(int id)
        {
            if (id <= 0)
                return null;

            var item = _itemRepository.Table.FirstOrDefault(i => i.Id == id);

            return item;
        }

        public int GetItemsCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetItemsQuery(sinceId).Count();
        }

        private IQueryable<Item> GetItemsQuery(int sinceId = 0)
        {
            var query = _itemRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}