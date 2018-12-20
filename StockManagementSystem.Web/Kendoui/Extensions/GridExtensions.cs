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

        // TODO: Grid server-side filter rendering
        public static IList<T> Filter<T>(this IEnumerable<T> source, string logic, IList<GridFilterDetails> filters)
        {
            if (string.IsNullOrWhiteSpace(logic))
                throw new ArgumentNullException(nameof(logic));

            string expression = String.Empty;
            //where(f => f.Name == 'John' && f.Name != 'Cena')

            for (var i = 0; i < filters.Count; i++)
            {
                logic = (i == filters.Count - 1) ? "" : logic;

                //expression += $"f.{filters[i].Filters} {BuildOperator(filters[i].Operator)}"
                expression += $"f => f.{filters[i].Field} {filters[i].Operator} {filters[i].Value} {logic} ";

               
            }

            //foreach (var filter in filters)
            //{
            //    if (!string.IsNullOrWhiteSpace(filter.Logic) && filter.Filters.Any())
            //    {
            //        source = source.ConstructFirstLevelExp(filter.Logic, filter.Filters);
            //    }

            //    expression += $"f => f.{filter.Field} {filter.Operator} {filter.Value} {logic} ";

            //}

            return source.ToList();
        }

        private static IEnumerable<T> ConstructFirstLevelExp<T>(this IEnumerable<T> source, string logic,
            IList<GridFilterDetails> filters)
        {
            string expression = String.Empty;
            foreach (var filter in filters)
            {
                expression += $"f => f.{filter.Field} {filter.Operator} {filter.Value}";
            }

            return source.ToList();
        }
    }
}