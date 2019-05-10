using System;
using System.Linq;
using System.Linq.Expressions;

namespace StockManagementSystem.Api.Extensions
{
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