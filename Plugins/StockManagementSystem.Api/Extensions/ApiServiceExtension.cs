using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using StockManagementSystem.Api.Helpers;

namespace StockManagementSystem.Api.Extensions
{
    public static class ApiServiceExtension
    {
        #region Private methods

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

        private static List<string> SplitQueryAttributes(string query)
        {
            var queries = query
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            return queries;
        }

        #endregion

        public static IQueryable<T> GetQuery<T>(this IQueryable<T> query, string sortColumn, bool descending, int sinceId = 0)
        {
            var parameter = Expression.Parameter(typeof(T), "p");

            string command = "OrderBy";

            if (descending)
                command = "OrderByDescending";

            //resolve snake_case
            sortColumn = sortColumn.Replace("_", string.Empty);

            var property = ReflectionHelper.GetPropertyInfo(ref sortColumn, typeof(T));
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
                var key = searchParam.Key;
                if (ReflectionHelper.GetPropertyInfo(ref key, typeof(T)) is PropertyInfo pi)
                {
                    LambdaExpression expression;

                    if (pi.PropertyType == typeof(string))
                    {
                        expression = DynamicExpressionParser.ParseLambda(typeof(T), typeof(bool),
                            $"{key} = @0 || {key}.Contains(@0)", searchParam.Value);
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
                            $"{key} >= @0 and {key} < @1", expObj);
                    }
                    else
                    {
                        expression = DynamicExpressionParser.ParseLambda(typeof(T), typeof(bool), $"{key} = @0", searchParam.Value);
                    }

                    query = query.Where("@0(it)", expression);
                }
            }

            return query;
        }

        public static Dictionary<string, string> EnsureSearchQueryIsValid(string query, Func<string, Dictionary<string, string>> resolveSearchQuery)
        {
            return !string.IsNullOrEmpty(query) ? resolveSearchQuery(query) : null;
        }

        public static Dictionary<string, string> ResolveSearchQuery(string query)
        {
            var searchQuery = new Dictionary<string, string>();

            var queryList = SplitQueryAttributes(query);
            if (queryList.Count == 0)
                return searchQuery;

            var fieldValueList = queryList.Select(q => Regex.Split(q, @"(\w+):").Where(s => !string.IsNullOrEmpty(s)).ToList()).ToList();
            foreach (var fields in fieldValueList)
            {
                if (fields.Count < 2)
                    continue;

                for (var i = 0; i < fields.Count; i += 2)
                {
                    var field = fields[i];
                    var value = fields[i + 1];

                    if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(value))
                    {
                        field = field.Replace("_", string.Empty);
                        searchQuery.Add(field.Trim(), value.Trim());
                    }
                }
            }

            return searchQuery;
        }
    }
}