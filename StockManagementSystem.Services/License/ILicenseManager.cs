using System.Threading.Tasks;
using StockManagementSystem.Core.Plugins;

namespace StockManagementSystem.Services.License
{
    public interface ILicenseManager : IPlugin
    {
        Task<string> GetPublicKey(string licenseToName, string licenseToEmail);

        Task SaveLicenseState(string serialNo);

        Task ResetLicenseInstance();
    }
}