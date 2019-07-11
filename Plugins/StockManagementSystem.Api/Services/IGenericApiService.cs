using System.Collections.Generic;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core;

namespace StockManagementSystem.Api.Services
{
    public interface IGenericApiService<T, E> where T : BaseDto where E : BaseEntity
    {
        IList<T> GetAll(
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false);

        T GetById(int id);

        int Count();

        Search<T> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false);
    }
}