using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Api.Extensions;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Directory;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Data;
using static StockManagementSystem.Api.Extensions.ApiServiceExtension;

namespace StockManagementSystem.Api.Services
{
    public class GenericApiService<T, E> : IGenericApiService<T, E> where T : BaseDto where E : BaseEntity
    {
        private readonly IUnitOfWork _worker;
        private readonly Type[] _lazyLoadEntities = {typeof(PermissionRoles), typeof(UserRole), typeof(UserStore), typeof(LocalState)};

        public GenericApiService(IUnitOfWork worker)
        {
            _worker = worker;
        }

        public async Task<IList<T>> GetAll(
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false)
        {
            var repo = _worker.GetRepositoryAsync<E>();

            var query = await repo.GetQueryableAsync(queryExp: src => src.GetQuery(sortColumn, @descending, sinceId), disableTracking: typeof(E).InitTracker(_lazyLoadEntities));

            return new ApiList<E>(query, page - 1, limit).Select(entity => entity.ToDto<T, E>()).ToList();
        }

        public async Task<T> GetById(int id)
        {
            if (id == 0)
                return null;

            var repo = _worker.GetRepositoryAsync<E>();

            var query = await repo.SingleAsync(src =>
                typeof(E) == typeof(Store) ? (src as Store).P_BranchNo == id : src.Id == id);

            return query.ToDto<T, E>();
        }

        public async Task<int> Count()
        {
            var repo = _worker.GetRepositoryAsync<E>();

            return (await repo.GetPagedListAsync()).Count;
        }

        public async Task<Search<T>> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false)
        {
            var repo = _worker.GetRepositoryAsync<E>();

            var query = await repo.GetQueryableAsync(searchFunc: src => src.SearchFunc(queryParams), queryExp: src => src.GetQuery(sortColumn, @descending), disableTracking: typeof(E).InitTracker(_lazyLoadEntities));

            var search = new SearchWrapper<T, E>();
            return count
                ? search.ToCount(query)
                : search.ToList(query, page, limit, list => list.Select(entity => entity.ToDto<T, E>()).ToList());
        }
    }

    internal static class Handler
    {
        public static IQueryable<TEntity> SearchFunc<TEntity>(this IQueryable<TEntity> query, string queryParams)
            where TEntity : BaseEntity
        {
            var searchParams = EnsureSearchQueryIsValid(queryParams, ResolveSearchQuery);
            return searchParams != null ? query.HandleSearchParams(searchParams) : query;
        }

        public static bool InitTracker<T>(this T obj, params T[] args)
        {
            return !args.Contains(obj);
        }
    }
}