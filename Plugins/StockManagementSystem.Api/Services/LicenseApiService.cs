using System;
using System.Linq;
using System.Threading.Tasks;
using Portable.Licensing;
using Portable.Licensing.Validation;
using StockManagementSystem.Services.License;
using StockManagementSystem.Services.Plugins;

namespace StockManagementSystem.Api.Services
{
    public class LicenseApiService : ILicenseApiService
    {
        private readonly IPluginService _pluginService;

        public LicenseApiService(IPluginService pluginService)
        {
            _pluginService = pluginService;
        }

        public async Task<ILicenseManager> LoadLicenseManager(string systemName)
        {
            var descriptor = _pluginService.GetPluginDescriptorBySystemName<ILicenseManager>(systemName);
            return await Task.FromResult(descriptor?.Instance<ILicenseManager>());
        }

        public async Task<string[]> ValidateLicense(string payload, string sno)
        {
            var licenseManager = await LoadLicenseManager("StockManagementSystem.LicenseManager");
            if (licenseManager == null)
                throw new ArgumentNullException(nameof(licenseManager));

            var license = License.Load(payload);
            if (license == null)
                throw new ArgumentNullException(nameof(licenseManager));

            var publicKey = await licenseManager.GetPublicKey(license.Customer.Name, license.Customer.Email);

            var validationResults = license
                .Validate()
                .AssertThat(lic => lic.ProductFeatures.Contains(sno), new GeneralValidationFailure {Message = $"Device with serial no {sno} not licensed."})
                .And()
                .Signature(publicKey)
                .AssertValidLicense().ToList();

            if (validationResults.Count == 0)
                await licenseManager.SaveLicenseState(sno);

            return validationResults.Count > 0 ? validationResults.Select(x => x.Message).ToArray() : null;
        }
    }
}