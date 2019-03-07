using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Stores
{
    public class StoreSearchModel : BaseSearchModel
    {
        public StoreSearchModel()
        {
            AvailableAreaCodes = new List<SelectListItem>();
            AvailableCities = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
        }

        [Display(Name = "Store name")]
        public string SearchStoreName { get; set; }

        [Display(Name = "Area code")]
        public string SearchAreaCode { get; set; }
        public IList<SelectListItem> AvailableAreaCodes { get; set; }

        [Display(Name = "City")]
        public string SearchCity { get; set; }
        public IList<SelectListItem> AvailableCities { get; set; }

        [Display(Name = "State")]
        public string SearchState { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
    }
}