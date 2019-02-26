using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Devices
{
    public class DeviceSearchModel : BaseSearchModel
    {
        public DeviceSearchModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [Display(Name = "Store")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [Display(Name = "Serial No.")]
        public string SearchSerialNo { get; set; }
    }
}