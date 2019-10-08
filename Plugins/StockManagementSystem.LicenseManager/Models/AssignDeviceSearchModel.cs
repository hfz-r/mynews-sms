using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.LicenseManager.Models
{
    public class AssignDeviceSearchModel : BaseSearchModel
    {
        public AssignDeviceSearchModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [Display(Name = "Serial No")]
        public string SearchSerialNo { get; set; }

        [Display(Name = "Store")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}