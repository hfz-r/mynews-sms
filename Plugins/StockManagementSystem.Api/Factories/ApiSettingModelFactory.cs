using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Domain;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.ApiSettings;
using StockManagementSystem.Api.Models.ApiSettings.Clients;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace StockManagementSystem.Api.Factories
{
    public class ApiSettingModelFactory : IApiSettingModelFactory
    {
        private readonly ISettingService _settingService;
        private readonly IClientService _clientService;
        private readonly ITenantContext _tenantContext;

        public ApiSettingModelFactory(
            ISettingService settingService, 
            IClientService clientService,
            ITenantContext tenantContext)
        {
            _settingService = settingService;
            _clientService = clientService;
            _tenantContext = tenantContext;
        }

        protected Task<ApiConfigurationModel> PrepareApiGeneralModel()
        {
            //load settings for a chosen tenant scope
            var tenantId = _tenantContext.ActiveTenantScopeConfiguration;
            var apiSettings = _settingService.LoadSetting<ApiSettings>(tenantId);
            
            //fill in model values from the entity
            var model = apiSettings.ToModel();

            //fill in additional values (not existing in the entity)
            model.ActiveTenantScopeConfiguration = tenantId;

            //fill in overridden values
            if (tenantId > 0)
            {
                model.EnableApi_OverrideForTenant =
                    _settingService.SettingExists(apiSettings, x => x.EnableApi, tenantId);
                model.EnableLogging_OverrideForTenant =
                    _settingService.SettingExists(apiSettings, x => x.EnableLogging, tenantId);
            }

            return Task.FromResult(model);
        }

        protected Task PrepareUrisSearchModel(UrisSearchModel searchModel, Client client)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            searchModel.ClientId = client.Id;

            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        public async Task<ApiModel> PrepareApiSettingsModel()
        {
            var model = new ApiModel {ApiConfiguration = await PrepareApiGeneralModel()};

            //prepare api client search model
            await PrepareApiClientSearchModel(model.ClientSearchModel);

            return model;
        }

        public Task<ClientSearchModel> PrepareApiClientSearchModel(ClientSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        public async Task<DataSourceResult> PrepareApiClientListModel(ClientSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var clients = await _clientService.GetAllClientsAsync();

            var model = new DataSourceResult
            {
                Data = clients,
                Total = clients.Count
            };

            return model;
        }

        public async Task<ClientModel> PrepareClientModel(ClientModel model, Client client, bool excludeProperties = false)
        {
            if (client != null)
            {
                model = model ?? client.ToModel();

                if (!excludeProperties)
                {
                    model.Enabled = client.Enabled;
                    model.JavaScriptClient = client.AllowedGrantTypes.Any(g => g.GrantType.Equals(OidcConstants.GrantTypes.Implicit));
                    model.ClientId = client.ClientId;
                    model.ClientName = client.ClientName;
                    model.ClientSecret = client.ClientSecrets.FirstOrDefault()?.Description;
                    model.AccessTokenLifetime = client.AccessTokenLifetime;
                    model.RefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime;
                }

                //prepare nested search models
                await PrepareUrisSearchModel(model.UrisSearchModel, client);
            }

            //set default values for the new model
            if (client == null)
            {
                model.Enabled = true;
                model.JavaScriptClient = false;
                model.ClientSecret = Guid.NewGuid().ToString();
                model.ClientId = Guid.NewGuid().ToString();
                model.AccessTokenLifetime = Configurations.DefaultAccessTokenExpiration;
                model.RefreshTokenLifetime = Configurations.DefaultRefreshTokenExpiration;
            }

            return model;
        }

        public async Task<RedirectUrisListModel> PrepareRedirectUrisListModel(UrisSearchModel searchModel, Client client)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var redirectUris = await _clientService.GetRedirectUris(
                clientId: client.Id,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new RedirectUrisListModel
            {
                Data = redirectUris.Select(re =>
                {
                    var model1 = re.ToModel();
                    model1.ClientId = re.ClientId;
                    model1.Url = re.RedirectUri;

                    return model1;
                }),
                Total = redirectUris.Count
            };

            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        public async Task<PostLogoutUrisListModel> PreparePostLogoutListModel(UrisSearchModel searchModel, Client client)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var postLogoutUris = await _clientService.GetPostLogoutUris(
                clientId: client.Id,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new PostLogoutUrisListModel
            {
                Data = postLogoutUris.Select(pl =>
                {
                    var model1 = pl.ToModel();
                    model1.ClientId = pl.ClientId;
                    model1.Url = pl.PostLogoutRedirectUri;

                    return model1;
                }),
                Total = postLogoutUris.Count
            };

            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        public async Task<CorsOriginUrisListModel> PrepareCorsOriginsListModel(UrisSearchModel searchModel, Client client)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            var corsOriginsUris = await _clientService.GetCostOriginsUris(
                clientId: client.Id,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new CorsOriginUrisListModel
            {
                Data = corsOriginsUris.Select(co =>
                {
                    var model1 = co.ToModel();
                    model1.ClientId = co.ClientId;
                    model1.Url = co.Origin;

                    return model1;
                }),
                Total = corsOriginsUris.Count
            };

            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }
    }
}