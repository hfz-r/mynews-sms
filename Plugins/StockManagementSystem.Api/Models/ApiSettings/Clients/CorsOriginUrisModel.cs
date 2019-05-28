using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Api.Models.ApiSettings.Clients
{
    public class CorsOriginUrisModel : BaseEntityModel
    {
        public int ClientId { get; set; }

        public string Url { get; set; }
    }
}