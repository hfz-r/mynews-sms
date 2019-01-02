using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Extensions
{
    public static class CommonExtensions
    {
        // In-memory paging of entities (models)
        public static IEnumerable<T> PagedForCommand<T>(this IEnumerable<T> current, DataSourceRequest command)
        {
            return current.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize);
        }

        // In-memory paging of objects
        public static IEnumerable<T> PaginationByRequestModel<T>(this IEnumerable<T> collection, IPagingRequestModel requestModel)
        {
            return collection.Skip((requestModel.Page - 1) * requestModel.PageSize).Take(requestModel.PageSize);
        }
    }
}