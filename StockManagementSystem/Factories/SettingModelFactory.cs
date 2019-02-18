using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Setting;
using StockManagementSystem.Models.Stores;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Stores;

namespace StockManagementSystem.Factories
{
    public class SettingModelFactory : ISettingModelFactory
    {
        private readonly IBaseModelFactory _baseModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public SettingModelFactory(
            IBaseModelFactory baseModelFactory, 
            IDateTimeHelper dateTimeHelper, 
            IGenericAttributeService genericAttributeService, 
            ISettingService settingService,
            IStoreService storeService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _baseModelFactory = baseModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _genericAttributeService = genericAttributeService;
            _settingService = settingService;
            _storeService = storeService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        public async Task<SettingModeModel> PrepareSettingModeModel(string modeName)
        {
            var model = new SettingModeModel
            {
                ModeName = modeName,
                Enabled = await _genericAttributeService.GetAttributeAsync<bool>(_workContext.CurrentUser, modeName)
            };

            return model;
        }

        /// <summary>
        /// Prepare store scope configuration model
        /// </summary>
        public async Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModel()
        {
            var storeScopeModel = new StoreScopeConfigurationModel
            {
                Stores = (await _storeService.GetStoresAsync()).Select(store =>
                {
                    var model = store.ToModel<StoreModel>();
                    model.Id = store.P_BranchNo;
                    model.Name = store.P_Name;

                    return model;

                }).ToList(),
                StoreId = _storeContext.ActiveStoreScopeConfiguration
            };

            return storeScopeModel;
        }
    }
}