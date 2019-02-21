using System;
using System.Threading.Tasks;
using StockManagementSystem.Api.Domain;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.ApiSettings;
using StockManagementSystem.Api.Models.ApiSettings.Clients;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Web.Kendoui;

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
    }
}