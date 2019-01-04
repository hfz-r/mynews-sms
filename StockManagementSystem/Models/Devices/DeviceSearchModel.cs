using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Devices
{
    public partial class DeviceSearchModel : BaseSearchModel
    {
        public DeviceSearchModel()
        {
            SelectedStoreId = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [Display(Name = "Store")]
        public IList<int> SelectedStoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
        
        [Display(Name = "Serial No.")]
        public string SearchSerialNo { get; set; }  
        
    }
}
