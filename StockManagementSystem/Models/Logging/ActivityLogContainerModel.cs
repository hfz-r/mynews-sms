using System.Collections.Generic;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Logging
{
    public class ActivityLogContainerModel : BaseModel
    {
        public ActivityLogContainerModel()
        {
            ListSignedIn = new SignedInLogSearchModel();
            ListLogs = new ActivityLogSearchModel();
            ListTypes = new List<ActivityLogTypeModel>();
        }

        public SignedInLogSearchModel ListSignedIn { get; set; }

        public ActivityLogSearchModel ListLogs { get; set; }

        public IList<ActivityLogTypeModel> ListTypes { get; set; }
    }
}