using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Management
{
    public class OutletManagementContainerModel : BaseModel
    {

        public OutletManagementContainerModel()
        {
            AssignUserList = new AssignUserSearchModel();
            GroupOutletList = new GroupOutletSearchModel();
        }

        public AssignUserSearchModel AssignUserList { get; set; }

        public GroupOutletSearchModel GroupOutletList { get; set; }
    }
}