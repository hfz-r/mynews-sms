using System.Collections.Generic;

namespace StockManagementSystem.Web.Kendoui
{
    public class GridFilter
    {
        public string Logic { get; set; }
        public IList<GridFilterDetails> Filters { get; set; }
    }

    public class GridFilterDetails
    {
        public string Operator { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public string Logic { get; set; }
        public IList<GridFilterDetails> Filters { get; set; }
    }
}