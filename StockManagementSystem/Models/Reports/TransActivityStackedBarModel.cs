using System.Collections.Generic;

namespace StockManagementSystem.Models.Reports
{
    public class TransActivityStackedBarModel
    {
        public object stacked { get; set; }

        public IList<DataSet> datasets { get; set; }
    }

    public class DataSet
    {
        public object label { get; set; }

        public object data { get; set; }
    }
}