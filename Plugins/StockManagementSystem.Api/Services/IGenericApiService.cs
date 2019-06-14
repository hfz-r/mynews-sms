using System;
using System.Collections.Generic;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DTOs;

namespace StockManagementSystem.Api.Services
{
    public interface IGenericApiService<T> where T : BaseDto
    {
        IList<T> GetEntities(
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false);

        T GetEntityById(int id);

        int GetEntityCount();

        IList<T> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false);
    }
}