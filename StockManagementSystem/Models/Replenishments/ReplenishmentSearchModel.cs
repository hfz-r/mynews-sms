using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Replenishments
{
    public class ReplenishmentSearchModel : BaseSearchModel
    {
        public ReplenishmentSearchModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [Display(Name = "Store")]
        public IList<int> SelectedStoreIds { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
    }
}
