using System.Collections.Generic;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.LicenseManager.Models
{
    public class AssignDeviceModel : BaseModel
    {
        public AssignDeviceModel()
        {
            SelectedDeviceIds = new List<int>();
        }

        public int LicenseId { get; set; }

        public IList<int> SelectedDeviceIds { get; set; }
    }
}