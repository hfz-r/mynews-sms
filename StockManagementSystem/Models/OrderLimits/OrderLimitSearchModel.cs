using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.OrderLimits
{
    public class OrderLimitSearchModel : BaseSearchModel
    {
        public OrderLimitSearchModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [Display(Name = "Store")]
        public IList<int> SelectedStoreIds { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        //Remove Percentage criteria; Not required - 05032019
        //[Display(Name = "Percentage")]
        //[DataType("Integer")]
        //public int? SearchPercentage { get; set; }

    }
}
