using System;
using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Items;

namespace StockManagementSystem.Api.Services
{
    public interface IItemApiService
    {
        Item GetItemById(int id);

        IList<Item> GetItems(int limit = 50, int page = 1, int sinceId = 0);

        int GetItemsCount();
    }
}