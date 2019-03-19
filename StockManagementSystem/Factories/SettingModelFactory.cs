using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Media;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Setting;
using StockManagementSystem.Models.Tenants;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Media;
using StockManagementSystem.Services.Tenants;

namespace StockManagementSystem.Factories
{
    public class SettingModelFactory : ISettingModelFactory
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly ITenantService _tenantService;
        private readonly IWorkContext _workContext;
        private readonly ITenantContext _tenantContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IBaseModelFactory _baseModelFactory;

        public SettingModelFactory(
            IGenericAttributeService genericAttributeService,
            ISettingService settingService,
            IPictureService pictureService,
            ITenantService tenantService,
            IWorkContext workContext,
            ITenantContext tenantContext,
            IDateTimeHelper dateTimeHelper,
            IBaseModelFactory baseModelFactory)
        {
            _genericAttributeService = genericAttributeService;
            _settingService = settingService;
            _pictureService = pictureService;
            _tenantService = tenantService;
            _workContext = workContext;
            _tenantContext = tenantContext;
            _dateTimeHelper = dateTimeHelper;
            _baseModelFactory = baseModelFactory;
        }

        #region Utilities

        protected Task<CommonSettingsModel> PrepareCommonSettingsModel()
        {
            var tenantId = _tenantContext.ActiveTenantScopeConfiguration;
            var commonSettings = _settingService.LoadSetting<CommonSettings>(tenantId);

            var model = new CommonSettingsModel
            {
                LogoPictureId = commonSettings.LogoPictureId,
                UseResponseCompression = commonSettings.UseResponseCompression,
            };

            if (tenantId <= 0)
                return Task.FromResult(model);

            //fill in overridden values
            model.LogoPictureId_OverrideForTenant = _settingService.SettingExists(commonSettings, x => x.LogoPictureId, tenantId);
            model.UseResponseCompression_OverrideForTenant = _settingService.SettingExists(commonSettings, x => x.UseResponseCompression, tenantId);

            return Task.FromResult(model);
        }

        protected Task<SecuritySettingsModel> PrepareSecuritySettingsModel()
        {
            var tenantId = _tenantContext.ActiveTenantScopeConfiguration;
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(tenantId);

            var model = new SecuritySettingsModel
            {
                EncryptionKey = securitySettings.EncryptionKey,
                ForceSslForAllPages = securitySettings.ForceSslForAllPages,
                EnableXsrfProtection = securitySettings.EnableXsrfProtection
            };

            if (securitySettings.AllowedIpAddresses != null)
                model.AllowedIpAddresses = string.Join(",", securitySettings.AllowedIpAddresses);

            return Task.FromResult(model);
        }

        protected Task<UserSettingsModel> PrepareUserSettingsModel()
        {
            var tenantId = _tenantContext.ActiveTenantScopeConfiguration;
            var userSettings = _settingService.LoadSetting<UserSettings>(tenantId);

            //fill in model values from the entity
            var model = userSettings.ToSettingsModel<UserSettingsModel>();

            return Task.FromResult(model);
        }

        protected async Task<DateTimeSettingsModel> PrepareDateTimeSettingsModel()
        {
            var tenantId = _tenantContext.ActiveTenantScopeConfiguration;
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(tenantId);

            var model = new DateTimeSettingsModel
            {
                AllowUsersToSetTimeZone = dateTimeSettings.AllowUsersToSetTimeZone,
                DefaultTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id,
            };

            //prepare available time zones
            await _baseModelFactory.PrepareTimeZones(model.AvailableTimeZones, false);

            return model;
        }

        #endregion

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
        /// Prepare tenant scope configuration model
        /// </summary>
        public async Task<TenantScopeConfigurationModel> PrepareTenantScopeConfigurationModel()
        {
            var model = new TenantScopeConfigurationModel
            {
                Tenants = (await _tenantService.GetTenantsAsync()).Select(t => t.ToModel<TenantModel>()).ToList(),
                TenantId = _tenantContext.ActiveTenantScopeConfiguration
            };

            return model;
        }

        public async Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModel()
        {
            var model = new GeneralCommonSettingsModel
            {
                ActiveTenantScopeConfiguration = _tenantContext.ActiveTenantScopeConfiguration,
                
                //prepare common settings model
                CommonSettings = await PrepareCommonSettingsModel(),

                //prepare security settings model
                SecuritySettings = await PrepareSecuritySettingsModel(),
            };

            return model;
        }

        public async Task<UserAdminSettingsModel> PrepareUserAdminSettingsModel()
        {
            var model = new UserAdminSettingsModel
            {
                ActiveTenantScopeConfiguration = _tenantContext.ActiveTenantScopeConfiguration,

                //prepare user settings model
                UserSettings = await PrepareUserSettingsModel(),

                //prepare date time settings model
                DateTimeSettings = await PrepareDateTimeSettingsModel(),
            };

            return model;
        }

        public Task<MediaSettingsModel> PrepareMediaSettingsModel()
        {
            var tenantId = _tenantContext.ActiveTenantScopeConfiguration;
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(tenantId);

            var model = mediaSettings.ToSettingsModel<MediaSettingsModel>();

            model.ActiveTenantScopeConfiguration = tenantId;
            model.PicturesStoredIntoDatabase = _pictureService.StoreInDb;

            if (tenantId <= 0)
                return Task.FromResult(model);

            //fill in overridden values
            model.AvatarPictureSize_OverrideForTenant = _settingService.SettingExists(mediaSettings, x => x.AvatarPictureSize, tenantId);
            model.MaximumImageSize_OverrideForTenant = _settingService.SettingExists(mediaSettings, x => x.MaximumImageSize, tenantId);
            model.DefaultImageQuality_OverrideForTenant = _settingService.SettingExists(mediaSettings, x => x.DefaultImageQuality, tenantId);

            return Task.FromResult(model);
        }
    }
}