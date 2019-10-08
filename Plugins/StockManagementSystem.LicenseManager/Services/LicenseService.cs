using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Portable.Licensing;
using Portable.Licensing.Security.Cryptography;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Media;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.LicenseManager.Infrastructure.Cache;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Events;
using StockManagementSystem.Services.Media;
using _ = Portable.Licensing.License;
using License = StockManagementSystem.LicenseManager.Domain.License;

namespace StockManagementSystem.LicenseManager.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<License> _licenseRepository;
        private readonly IRepository<DeviceLicense> _deviceLicenseRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IDownloadService _downloadService;
        private readonly IDeviceService _deviceService;

        private _ _activeLicense;

        public LicenseService(
            IEventPublisher eventPublisher,
            IRepository<Store> storeRepository,
            IRepository<License> licenseRepository,
            IRepository<DeviceLicense> deviceLicenseRepository,
            ICacheManager cacheManager,
            IDownloadService downloadService,
            IDeviceService deviceService)
        {
            _eventPublisher = eventPublisher;
            _storeRepository = storeRepository;
            _licenseRepository = licenseRepository;
            _deviceLicenseRepository = deviceLicenseRepository;
            _cacheManager = cacheManager;
            _downloadService = downloadService;
            _deviceService = deviceService;
        }

        #region Utilities

        protected async Task<Dictionary<string, string>> GetProductFeatures(IQueryable<DeviceLicense> deviceLicenses)
        {
            var dictionary = new Dictionary<string, string>();
            await deviceLicenses.ForEachAsync(l => dictionary.Add(l.SerialNo, "active"));

            return dictionary;
        }

        protected async Task<Download> InitGenerateLicense(License license, Func<License, bool> generateFunc)
        {
            var keyGenerator = KeyGenerator.Create();
            var keyPair = keyGenerator.GenerateKeyPair();

            license.PublicKey = keyPair.ToPublicKeyString();
            license.PrivateKey = keyPair.ToEncryptedPrivateKeyString(license.PassPhrase);

            var deviceLicenses = _deviceLicenseRepository.Table.Where(dl => dl.LicenseId == license.Id);

            license.Quantity = deviceLicenses.Count();
            license.ProductFeatures = await GetProductFeatures(deviceLicenses);

            var isGenerate = generateFunc(license);
            if (!isGenerate)
                throw new DefaultException("Failed to generate license.");
            if (_activeLicense == null)
                throw new DefaultException("Generated license not found.");

            var xmlElement = XElement.Parse(_activeLicense.ToString(), LoadOptions.None);
            var fileBinary = await _downloadService.GetDownloadBits(xmlElement, CancellationToken.None);

            var download = new Download
            {
                DownloadBinary = fileBinary,
                ContentType = MimeTypes.TextXml,
                Filename = $"license-{Regex.Replace(license.LicenseToName, @"\s+", "").ToLowerInvariant()}",
                Extension = ".lic"
            };
            await _downloadService.InsertDownload(download);

            //sync license with download
            license.DownloadId = download.Id;
            await UpdateLicense(license);

            return await Task.FromResult(download);
        }

        #endregion

        public async Task<IPagedList<License>> GetAllLicenses(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(ModelCacheEventConsumer.LICENSE_ALL_KEY, pageIndex, pageSize);
            return await _cacheManager.Get(key, async () =>
            {
                var query = from l in _licenseRepository.Table
                    orderby l.Id, l.LicenseToName
                    select l;
                var records = new PagedList<License>(query, pageIndex, pageSize);
                return await Task.FromResult(records);
            });
        }

        public async Task<License> GetLicenseById(int licenseId)
        {
            if (licenseId == 0)
                return await Task.FromResult<License>(null);

            return await _licenseRepository.GetByIdAsync(licenseId);
        }

        public async Task InsertLicense(License license)
        {
            if (license == null)
                throw new ArgumentNullException(nameof(license));

            await _licenseRepository.InsertAsync(license);

            _eventPublisher.EntityInserted(license);
        }

        public async Task UpdateLicense(License license)
        {
            if (license == null)
                throw new ArgumentNullException(nameof(license));

            await _licenseRepository.UpdateAsync(license);

            _eventPublisher.EntityUpdated(license);
        }

        public async Task DeleteLicense(License license)
        {
            if (license == null)
                throw new ArgumentNullException(nameof(license));

            var deviceLicenses = _deviceLicenseRepository.Table.Where(dl => dl.LicenseId == license.Id);
            //remove singleton item
            var context = SingletonList<string>.Instance;
            await deviceLicenses.ForEachAsync(dl => context.Remove(dl.SerialNo));
            //remove child
            await _deviceLicenseRepository.DeleteAsync(deviceLicenses);
            //remove parent
            await _licenseRepository.DeleteAsync(license);
            //remove download item
            var download = await _downloadService.GetDownloadById(license.DownloadId);
            await _downloadService.DeleteDownload(download);

            _eventPublisher.EntityDeleted(license);
        }

        public async Task GenerateLicense(int id)
        {
            var license = await GetLicenseById(id);
            if (!string.IsNullOrEmpty(license.PublicKey) && !string.IsNullOrEmpty(license.PrivateKey))
                throw new DefaultException("License already generated.");

            var download = await InitGenerateLicense(license, lic =>
            {
                _activeLicense = _.New()
                    .WithUniqueIdentifier(lic.LicenseId)
                    .As((LicenseType) lic.LicenseTypeId)
                    .WithMaximumUtilization(lic.Quantity)
                    .WithProductFeatures(lic.ProductFeatures)
                    .LicensedTo(lic.LicenseToName, lic.LicenseToEmail)
                    .ExpiresAt(lic.ExpirationDate)
                    .CreateAndSignWithPrivateKey(lic.PrivateKey, lic.PassPhrase);

                return !string.IsNullOrEmpty(_activeLicense?.Signature) &&  _activeLicense.VerifySignature(lic.PublicKey);
            });

            if (download == null)
                throw new ArgumentNullException(nameof(download));

            await Task.CompletedTask;
        }

        #region DeviceLicense

        public async Task<IPagedList<Device>> GetDeviceLicense(int? licenseId = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var devices = (await _deviceService.GetAllDevicesAsync()).AsQueryable();

            if (licenseId.HasValue)
                devices = devices.Where(d => d.DeviceLicenses.Any(mapping => mapping.LicenseId == licenseId.Value));

            devices = devices.OrderBy(d => d.SerialNo);

            return new PagedList<Device>(devices, pageIndex, pageSize);
        }

        public async Task<IQueryable<DeviceLicense>> GetDeviceLicenseByLicenseId(int licenseId)
        {
            if (licenseId == 0)
                throw new ArgumentNullException(nameof(licenseId));

            var deviceLicense = _deviceLicenseRepository.Table.Where(dl => dl.LicenseId == licenseId);

            return await Task.FromResult(deviceLicense);
        }

        //get DeviceLicense intersect
        public async Task<IPagedList<Device>> GetAssignedDevices(
            int[] storeIds = null,
            string serialNo = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var query = (await _deviceService.GetAllDevicesAsync())
                .Where(d => !_deviceLicenseRepository.Table.Any(dl => d.SerialNo == dl.SerialNo)).AsQueryable();

            if (storeIds != null && storeIds.Length > 0)
            {
                query = query.Join(_storeRepository.Table, x => x.StoreId, y => y.P_BranchNo,
                        (x, y) => new { Device = x, Store = y })
                    .Where(z => storeIds.Contains(z.Store.P_BranchNo))
                    .Select(z => z.Device)
                    .Distinct();
            }
            if (!string.IsNullOrWhiteSpace(serialNo))
                query = query.Where(u => u.SerialNo.Contains(serialNo));

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return new PagedList<Device>(query, pageIndex, pageSize);
        }

        #endregion
    }
}