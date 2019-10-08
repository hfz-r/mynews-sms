using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.LicenseManager.Domain;

namespace StockManagementSystem.LicenseManager.Services
{
    public interface ILicenseService
    {
        Task DeleteLicense(License license);

        Task<IPagedList<License>> GetAllLicenses(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<License> GetLicenseById(int licenseId);

        Task InsertLicense(License license);

        Task UpdateLicense(License license);

        Task GenerateLicense(int id);

        Task<IPagedList<Device>> GetDeviceLicense(int? licenseId = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IQueryable<DeviceLicense>> GetDeviceLicenseByLicenseId(int licenseId);

        Task<IPagedList<Device>> GetAssignedDevices(int[] storeIds = null, string serialNo = null, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}