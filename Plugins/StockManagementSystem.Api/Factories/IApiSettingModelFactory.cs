using System.Threading.Tasks;
using StockManagementSystem.Api.Models.ApiSettings;
using StockManagementSystem.Api.Models.ApiSettings.Clients;
using StockManagementSystem.Web.Kendoui;

namespace StockManagementSystem.Api.Factories
{
    public interface IApiSettingModelFactory
    {
        Task<ClientSearchModel> PrepareApiClientSearchModel(ClientSearchModel searchModel);

        Task<ApiModel> PrepareApiSettingsModel();

        Task<DataSourceResult> PrepareApiClientListModel(ClientSearchModel searchModel);
    }
}