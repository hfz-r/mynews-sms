using StockManagementSystem.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Models.Setting
{
    public class MapDeviceModel : BaseEntityModel
    {
        [Display(Name = "Serial Number")]
        public string SerialNo { get; set; }

        [Display(Name = "Model Number")]
        public string ModelNo { get; set; }

        [Display(Name = "Store")]
        public string StoreName { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
        
        [Display(Name = "Store")]
        public int SelectedStoreId { get; set; }
    }
}
