using StockManagementSystem.Web.Models;

namespace StockManagementSystem.LicenseManager.Models
{
    public class DeviceLicenseModel : BaseEntityModel
    {
        public string SerialNo { get; set; }

        public string ModelNo { get; set; }

        public string StoreName { get; set; }

        public string Status { get; set; }
    }
}