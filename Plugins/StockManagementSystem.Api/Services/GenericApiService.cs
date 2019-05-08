using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Transactions;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Api.Services
{
    public class GenericApiService<T> : IGenericApiService<T> where T : BaseDto
    {
        #region Private methods

        private object GetInstance()
        {
            var className = typeof(T).Name.Remove(typeof(T).Name.Length - 3);

            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .First(t => string.Equals(t.Name, className, StringComparison.Ordinal));

            return type != null ? Activator.CreateInstance(type) : null;
        }

        #endregion

        public IList<T> GetEntities(
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false)
        {
            var instance = GetInstance();

            if (instance is Transaction)
            {
                var repository = EngineContext.Current.Resolve<IRepository<Transaction>>();

                var query = repository.Table;
                query = query.GetQueryDynamic(sortColumn, descending, sinceId);

                var lst = new ApiList<Transaction>(query, page - 1, limit);

                return lst.Select(trans => trans.ToDto()).ToList() as IList<T>;
            }
            else if(instance is ShelfLocation)
            {
                var repository = EngineContext.Current.Resolve<IRepository<ShelfLocation>>();

                var query = repository.Table;
                query = query.GetQueryDynamic(sortColumn, descending, sinceId);

                var lst = new ApiList<ShelfLocation>(query, page - 1, limit);

                return lst.Select(sl => sl.ToDto()).ToList() as IList<T>;
            }
            else if (instance is TransporterTransaction)
            {
                var repository = EngineContext.Current.Resolve<IRepository<TransporterTransaction>>();

                var query = repository.Table;
                query = query.GetQueryDynamic(sortColumn, descending, sinceId);

                var lst = new ApiList<TransporterTransaction>(query, page - 1, limit);

                return lst.Select(t => t.ToDto()).ToList() as IList<T>;
            }

            return null;
        }

        public T GetEntityById(int id)
        {
            if (id == 0)
                return null;

            var instance = GetInstance();

            if (instance is Transaction)
            {
                var repository = EngineContext.Current.Resolve<IRepository<Transaction>>();

                var entity = repository.Table.FirstOrDefault(r => r.Id == id);

                return entity.ToDto() as T;
            }
            else if(instance is ShelfLocation)
            {
                var repository = EngineContext.Current.Resolve<IRepository<ShelfLocation>>();

                var entity = repository.Table.FirstOrDefault(r => r.Id == id);

                return entity.ToDto() as T;
            }
            else if (instance is TransporterTransaction)
            {
                var repository = EngineContext.Current.Resolve<IRepository<TransporterTransaction>>();

                var entity = repository.Table.FirstOrDefault(r => r.Id == id);

                return entity.ToDto() as T;
            }

            return null;
        }

        public int GetEntityCount()
        {
            var instance = GetInstance();

            if (instance is Transaction)
            {
                var repository = EngineContext.Current.Resolve<IRepository<Transaction>>();

                return repository.Table.Count();
            }
            else if (instance is ShelfLocation)
            {
                var repository = EngineContext.Current.Resolve<IRepository<ShelfLocation>>();

                return repository.Table.Count();
            }
            else if (instance is TransporterTransaction)
            {
                var repository = EngineContext.Current.Resolve<IRepository<TransporterTransaction>>();

                return repository.Table.Count();
            }

            return 0;
        }
    }

    public static class GenericApiServiceExtension
    {
        private static MethodCallExpression GetSinceIdQuery<T>(IQueryable<T> query, ParameterExpression parameter, int sinceId = 0)
        {
            if (sinceId > 0)
            {
                var property = Expression.Property(parameter, "Id");
                var constant = Expression.Constant(sinceId);
                var expression = Expression.GreaterThan(property, constant);

                // query.Where(p => p.CreatedOnUtc > value)
                MethodCallExpression whereCallExpression = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new[] { typeof(T) },
                    query.Expression,
                    Expression.Lambda<Func<T, bool>>(expression, parameter));

                return whereCallExpression;
            }

            return null;
        }

        public static IQueryable<T> GetQueryDynamic<T>(this IQueryable<T> query, string sortColumn, bool descending, int sinceId = 0)
        {
            var parameter = Expression.Parameter(typeof(T), "p");

            string command = "OrderBy";

            if (descending)
                command = "OrderByDescending";

            var property = typeof(T).GetProperty(sortColumn);
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            MethodCallExpression orderByCallExpression = Expression.Call(
                typeof(Queryable), 
                command, 
                new[] { typeof(T), property.PropertyType },
                GetSinceIdQuery(query, parameter, sinceId) ?? query.Expression,
                Expression.Quote(orderByExpression));

            return query.Provider.CreateQuery<T>(orderByCallExpression);
        }
    }
}