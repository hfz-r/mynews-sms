using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
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

        Task<ClientModel> PrepareClientModel(ClientModel model, Client client, bool excludeProperties = false);

        Task<RedirectUrisListModel> PrepareRedirectUrisListModel(UrisSearchModel searchModel, Client client);

        Task<PostLogoutUrisListModel> PreparePostLogoutListModel(UrisSearchModel searchModel, Client client);

        Task<CorsOriginUrisListModel> PrepareCorsOriginsListModel(UrisSearchModel searchModel, Client client);
    }
}