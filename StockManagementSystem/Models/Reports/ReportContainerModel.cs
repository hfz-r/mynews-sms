using StockManagementSystem.Models.Logging;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Reports
{
    public class ReportContainerModel : BaseModel
    {
        public ReportContainerModel()
        {
            ListSignedIn = new SignedInLogSearchModel();
            ListTransActivity = new TransActivitySearchModel();
        }

        public SignedInLogSearchModel ListSignedIn { get; set; }

        public TransActivitySearchModel ListTransActivity { get; set; }
    }
}