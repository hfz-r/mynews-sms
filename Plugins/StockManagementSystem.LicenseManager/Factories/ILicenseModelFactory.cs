using System.Threading.Tasks;
using StockManagementSystem.LicenseManager.Domain;
using StockManagementSystem.LicenseManager.Models;
using StockManagementSystem.Web.Kendoui;

namespace StockManagementSystem.LicenseManager.Factories
{
    public interface ILicenseModelFactory
    {
        Task<ConfigurationModel> PrepareLicenseConfigurationModel(ConfigurationModel searchModel);

        Task<DataSourceResult> PrepareLicenseListModel(ConfigurationModel searchModel);

        Task<LicenseModel> PrepareLicenseModel(LicenseModel model, License license, bool excludeProperties = false);

        Task<DataSourceResult> PrepareDeviceLicenseListModel(DeviceLicenseSearchModel searchModel, License license);

        Task<AssignDeviceSearchModel> PrepareAssignDeviceSearchModel(AssignDeviceSearchModel searchModel);

        Task<AssignDeviceListModel> PrepareAssignDeviceListModel(AssignDeviceSearchModel searchModel);
    }
}