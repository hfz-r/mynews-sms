using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Api.Extensions;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Directory;
using StockManagementSystem.Core.Domain.Master;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Data;
using StockManagementSystem.Services.Logging;
using static StockManagementSystem.Api.Extensions.ApiServiceExtension;

namespace StockManagementSystem.Api.Services
{
    public class GenericApiService<T, E> : IGenericApiService<T, E> where T : BaseDto where E : BaseEntity
    {
        private readonly IDbContext _dbContext;

        public GenericApiService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Private methods

        private IQueryable<E> GetList()
        {
            // Get the generic type definition
            MethodInfo method = typeof(IDbContext).GetMethod(nameof(IDbContext.Set), BindingFlags.Public | BindingFlags.Instance);

            // Build a method with the specific type argument
            method = method.MakeGenericMethod(typeof(E));

            var instance = method.Invoke(_dbContext, null) as IQueryable<E>;

            return instance;
        }

        private E Get(int id)
        {
            var query = GetList();

            var expression = typeof(E) == typeof(Store)
                ? DynamicExpressionParser.ParseLambda(typeof(Store), typeof(bool), "P_BranchNo == @0", id)
                : DynamicExpressionParser.ParseLambda(typeof(E), typeof(bool), "Id == @0", id);

            query = query.Where("@0(it)", expression);

            return query.First();
        }

        #endregion

        public IList<T> GetAll(
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false)
        {
            var query = GetList();

            query = query.GetQuery(sortColumn, @descending, sinceId);

            return new ApiList<E>(query, page - 1, limit).Select(entity => entity.ToDto<T, E>()).ToList();
        }

        public T GetById(int id)
        {
            if (id == 0)
                return null;

            var query = Get(id);

            return query.ToDto<T, E>();
        }

        public int Count()
        {
            var query = GetList();

            return (query ?? throw new InvalidOperationException()).Count();
        }

        public Search<T> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false)
        {
            var query = GetList();

            var searchParams = EnsureSearchQueryIsValid(queryParams, ResolveSearchQuery);
            if (searchParams != null)
                query = query.HandleSearchParams(searchParams);

            query = query.GetQuery(sortColumn, @descending);

            var _ = new SearchWrapper<T, E>();
            return count
                ? _.ToCount(query)
                : _.ToList(query, page, limit, list => list.Select(entity => entity.ToDto<T, E>()).ToList());
        }
    }
}