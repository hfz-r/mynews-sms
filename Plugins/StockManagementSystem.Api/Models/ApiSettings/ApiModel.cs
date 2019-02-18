using StockManagementSystem.Api.Models.ApiSettings.Clients;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Api.Models.ApiSettings
{
    public class ApiModel
    {
        public ApiModel()
        {
            ApiConfiguration = new ApiConfigurationModel();
            ClientSearchModel = new ClientSearchModel();
        }

        public ApiConfigurationModel ApiConfiguration { get; set; }

        public ClientSearchModel ClientSearchModel { get; set; }
    }
}