using System.Threading.Tasks;
using StockManagementSystem.Models.Setting;

namespace StockManagementSystem.Factories
{
    public interface ISettingModelFactory
    {
        Task<SettingModeModel> PrepareSettingModeModel(string modeName);

        Task<TenantScopeConfigurationModel> PrepareTenantScopeConfigurationModel();
    }
}