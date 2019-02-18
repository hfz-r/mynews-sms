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
        private readonly IStoreContext _storeContext;

        public ApiSettingModelFactory(
            ISettingService settingService, 
            IClientService clientService,
            IStoreContext storeContext)
        {
            _settingService = settingService;
            _clientService = clientService;
            _storeContext = storeContext;
        }

        protected Task<ApiConfigurationModel> PrepareApiGeneralModel()
        {
            //load settings for a chosen store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var apiSettings = _settingService.LoadSetting<ApiSettings>(storeId);
            
            //fill in model values from the entity
            var model = apiSettings.ToModel();

            //fill in additional values (not existing in the entity)
            model.ActiveStoreScopeConfiguration = storeId;

            //fill in overridden values
            if (storeId > 0)
            {
                model.EnableApi_OverrideForStore =
                    _settingService.SettingExists(apiSettings, x => x.EnableApi, storeId);
                model.EnableLogging_OverrideForStore =
                    _settingService.SettingExists(apiSettings, x => x.EnableLogging, storeId);
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