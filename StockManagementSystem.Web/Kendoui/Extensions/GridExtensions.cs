using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace StockManagementSystem.Web.Kendoui.Extensions
{
    public static class GridExtensions
    {
        public static async Task<IList<T>> Sort<T>(this IEnumerable<T> source, string field, string dir)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));

            if (string.IsNullOrWhiteSpace(dir))
                throw new ArgumentNullException(nameof(dir));

            var filter = "order => order." + field;
            var options = ScriptOptions.Default.AddReferences(typeof(T).Assembly);

            Func<T, object> exp = await CSharpScript.EvaluateAsync<Func<T, object>>(filter, options);

            var result = (dir == "asc") ? source.OrderBy(exp) : source.OrderByDescending(exp);
            return result.ToList();
        }

        public static async Task<IList<T>> Filter<T>(this IEnumerable<T> source, GridFilter filter)
        {
            const string selector = "f.";

            string expression = "f=>";
            var filters = filter.Filters;
            for (var i = 0; i < filters.Count; i++)
            {
                var f = filters[i];
                if (f.Filters == null)
                {
                    if (i == 0)
                        expression += $"{ConstructExpression<T>(f, selector)}" + " ";
                    if (i != 0)
                        expression += $"{Operator(filter.Logic)} {ConstructExpression<T>(f, selector)}" + " ";
                    if (i == filters.Count - 1)
                    {
                        CleanUp(ref expression);
                        source = await source.ProcessExpression(expression);
                    }
                }
                else
                {
                    source = await Filter<T>(source, f);
                }
            }

            return source.ToList();
        }

        private static string ConstructExpression<T>(GridFilter filter, string selector)
        {
            var type = typeof(T);
            var property = type.GetProperty(filter.Field);

            switch (filter.Operator.ToLowerInvariant())
            {
                case "eq":
                case "neq":
                case "gte":
                case "gt":
                case "lte":
                case "lt":
                    if (typeof(DateTime).IsAssignableFrom(property?.PropertyType))
                    {
                        var value = DateTime.Parse(filter.Value).Date;
                        return $"EntityFunctions.TruncateTime(({NullChecker(selector, filter.Field)} {selector}{filter.Field}) {Operator(filter.Operator)} \"{value}\")";
                    }
                    if (typeof(int).IsAssignableFrom(property?.PropertyType))
                    {
                        var value = int.Parse(filter.Value);
                        return $"({NullChecker(selector, filter.Field)} {selector}{filter.Field} {Operator(filter.Operator)} {value})";
                    }

                    return $"({NullChecker(selector, filter.Field)} {selector}{filter.Field} {Operator(filter.Operator)} \"{filter.Value}\")";

                case "startswith":
                    return $"({NullChecker(selector, filter.Field)} {selector}{filter.Field}.StartsWith(\"{filter.Value}\"))";
                case "endswith":
                    return $"({NullChecker(selector, filter.Field)} {selector}{filter.Field}.EndsWith(\"{filter.Value}\"))";
                case "contains":
                    return $"({NullChecker(selector, filter.Field)} {selector}{filter.Field}.Contains(\"{filter.Value}\"))";
                case "doesnotcontain":
                    return $"({NullChecker(selector, filter.Field)} !{selector}{filter.Field}.Contains(\"{filter.Value}\"))";
                default:
                    throw new ArgumentException("This operator is not yet supported for this Grid", filter.Operator);
            }
        }

        private static void CleanUp(ref string expression)
        {
            // clean up f=>
            switch (expression.Trim().Substring(3, 2).ToLower())
            {
                case "&&":
                    expression = expression.Trim().Remove(3, 2);
                    break;
                case "||":
                    expression = expression.Trim().Remove(3, 2);
                    break;
            }
        }

        private static string Operator(string @operator)
        {
            switch (@operator.ToLower())
            {
                case "eq": return "==";
                case "neq": return "!=";
                case "gte": return ">=";
                case "gt": return ">";
                case "lte": return "<=";
                case "lt": return "<";
                case "or": return "||";
                case "and": return "&&";
                default: return null;
            }
        }

        private static string NullChecker(string selector, string field)
        {
            return $"{selector}{field} != null && ";
        }

        private static async Task<IEnumerable<T>> ProcessExpression<T>(this IEnumerable<T> source, string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentNullException(nameof(expression));

            var options = ScriptOptions.Default.AddReferences(typeof(T).Assembly);
            Func<T, bool> exp = await CSharpScript.EvaluateAsync<Func<T, bool>>(expression, options);

            return source.Where(exp);
        }
    }
}