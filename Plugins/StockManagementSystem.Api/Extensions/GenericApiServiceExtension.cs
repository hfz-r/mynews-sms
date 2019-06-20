using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using StockManagementSystem.Api.Helpers;

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

            var property = typeof(T).GetProperty(sortColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
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

        public static IQueryable<T> HandleSearchParams<T>(this IQueryable<T> query, IReadOnlyDictionary<string, string> searchParams)
        {
            foreach (var searchParam in searchParams)
            {
                if (ReflectionHelper.GetPropertyInfo(searchParam.Key, typeof(T)) is PropertyInfo pi)
                {
                    LambdaExpression expression;

                    if (pi.PropertyType == typeof(string))
                    {
                        expression = DynamicExpressionParser.ParseLambda(typeof(T), typeof(bool),
                            $"{searchParam.Key} = @0 || {searchParam.Key}.Contains(@0)", searchParam.Value);
                    }
                    else if (pi.PropertyType == typeof(DateTime))
                    {
                        var today = DateTime.Now;
                        var dateToCompare = today.AddDays(Convert.ToDouble(searchParam.Value));

                        object[] expObj = null;
                        if (Convert.ToDouble(searchParam.Value) > 0)
                            expObj = new object[] {today, dateToCompare};
                        else if (Convert.ToDouble(searchParam.Value) < 0)
                            expObj = new object[] {dateToCompare, today};

                        expression = DynamicExpressionParser.ParseLambda(typeof(T), typeof(bool),
                            $"{searchParam.Key} >= @0 and {searchParam.Key} < @1", expObj);
                    }
                    else
                    {
                        expression = DynamicExpressionParser.ParseLambda(typeof(T), typeof(bool),
                            $"{searchParam.Key} = @0", searchParam.Value);
                    }

                    query = query.Where("@0(it)", expression);
                }
            }

            return query;
        }
    }
}