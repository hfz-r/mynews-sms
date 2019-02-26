using System;
using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Items;

namespace StockManagementSystem.Api.Services
{
    public interface IItemApiService
    {
        Item GetItemById(int id);

        IList<Item> GetItems(int? groupId = null, int? vendorId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = 50, int page = 1, int sinceId = 0);

        int GetItemsCount(int? groupId = null, int? vendorId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = 50, int page = 1, int sinceId = 0);
    }
}