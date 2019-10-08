using System.Threading.Tasks;
using StockManagementSystem.Services.License;

namespace StockManagementSystem.Api.Services
{
    public interface ILicenseApiService
    {
        Task<ILicenseManager> LoadLicenseManager(string systemName);

        Task<string[]> ValidateLicense(string payload, string id);
    }
}