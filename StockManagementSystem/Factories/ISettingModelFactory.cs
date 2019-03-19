using System.Threading.Tasks;
using StockManagementSystem.Models.Setting;

namespace StockManagementSystem.Factories
{
    public interface ISettingModelFactory
    {
        Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModel();

        Task<UserAdminSettingsModel> PrepareUserAdminSettingsModel();

        Task<MediaSettingsModel> PrepareMediaSettingsModel();

        Task<SettingModeModel> PrepareSettingModeModel(string modeName);

        Task<TenantScopeConfigurationModel> PrepareTenantScopeConfigurationModel();
    }
}