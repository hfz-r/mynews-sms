namespace StockManagementSystem.Web.Models
{
    public class ActionAlertModel : BaseEntityModel
    {
        public string WindowId { get; set; }

        public string AlertId { get; set; }

        public string AlertMessage { get; set; }
    }
}