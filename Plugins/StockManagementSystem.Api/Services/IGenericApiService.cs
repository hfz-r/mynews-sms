using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core;

namespace StockManagementSystem.Api.Services
{
    public interface IGenericApiService<T, E> where T : BaseDto where E : BaseEntity
    {
        Task<IList<T>> GetAll(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId, string sortColumn = Configurations.DefaultOrder, bool descending = false);
        Task<T> GetById(int id);
        Task<int> Count();
        Task<Search<T>> Search(string queryParams = "", int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, string sortColumn = Configurations.DefaultOrder, bool descending = false, bool count = false);
    }
}