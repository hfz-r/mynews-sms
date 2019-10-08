using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portable.Licensing;
using StockManagementSystem.Core;
using StockManagementSystem.Factories;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.LicenseManager.Models;
using StockManagementSystem.LicenseManager.Services;
using StockManagementSystem.Models.Devices;
using StockManagementSystem.Services;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;
using License = StockManagementSystem.LicenseManager.Domain.License;

namespace StockManagementSystem.LicenseManager.Factories
{
    public class LicenseModelFactory : ILicenseModelFactory
    {
        private readonly IBaseModelFactory _baseModelFactory;
        private readonly ILicenseService _licenseService;

        public LicenseModelFactory(
            IBaseModelFactory baseModelFactory,
            ILicenseService licenseService)
        {
            _baseModelFactory = baseModelFactory;
            _licenseService = licenseService;
        }

        #region Utilities

        private Task PrepareLicenseType(IList<SelectListItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var types = LicenseType.Standard.ToSelectList(false);
            foreach (var t in types)
            {
                items.Add(t);
            }

            return Task.CompletedTask;
        }

        private Task PrepareFeatureSearchModel(DeviceLicenseSearchModel searchModel, License license)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (license == null)
                throw new ArgumentNullException(nameof(license));

            searchModel.LicenseId = license.Id;
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        #endregion

        public Task<ConfigurationModel> PrepareLicenseConfigurationModel(ConfigurationModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        public async Task<DataSourceResult> PrepareLicenseListModel(ConfigurationModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var records = await _licenseService.GetAllLicenses(
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var result = new DataSourceResult
            {
                Data = records.Select(license =>
                {
                    var model = license.ToModel<LicenseModel>();
                    model.CountDevices = _licenseService.GetDeviceLicenseByLicenseId(model.Id).GetAwaiter().GetResult().Count();
                    model.LicenseType = CommonHelper.ConvertEnum(license.LicenseType.ToString());
                    model.Generated = model.DownloadId > 0;

                    return model;
                }),
                Total = records.Count
            };

            return result;
        }

        public async Task<LicenseModel> PrepareLicenseModel(LicenseModel model, License license, bool excludeProperties = false)
        {
            if (license != null)
            {
                model = model ?? license.ToModel<LicenseModel>();

                if (!excludeProperties)
                {
                    model.CountDevices = (await _licenseService.GetDeviceLicenseByLicenseId(model.Id)).Count();
                    model.LicenseType = CommonHelper.ConvertEnum(license.LicenseType.ToString());
                    model.Generated = model.DownloadId > 0;
                }

                await PrepareFeatureSearchModel(model.DeviceLicenseSearchModel, license);
            }

            if (license == null)
                model.Generated = false;

            await PrepareLicenseType(model.AvailableLicenseType);

            return model;
        }

        #region DeviceLicense

        public async Task<DataSourceResult> PrepareDeviceLicenseListModel(DeviceLicenseSearchModel searchModel, License license)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (license == null)
                throw new ArgumentNullException(nameof(license));

            var deviceLicense = await _licenseService.GetDeviceLicense(
                licenseId: license.Id,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var result = new DataSourceResult
            {
                Data = deviceLicense.Select(device =>
                {
                    var model = device.ToModel<DeviceLicenseModel>();
                    model.StoreName = device.Store != null ? device.Store.P_BranchNo + " - " + device.Store.P_Name : string.Empty;

                    return model;
                }),
                Total = deviceLicense.TotalCount
            };

            return result;
        }

        public async Task<AssignDeviceSearchModel> PrepareAssignDeviceSearchModel(AssignDeviceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            await _baseModelFactory.PrepareStores(searchModel.AvailableStores);

            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        public async Task<AssignDeviceListModel> PrepareAssignDeviceListModel(AssignDeviceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var devices = await _licenseService.GetAssignedDevices(
                storeIds: searchModel.SelectedStoreIds.ToArray(),
                serialNo: searchModel.SearchSerialNo,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new AssignDeviceListModel
            {
                Data = devices.Select(device =>
                {
                    var devicesModel = device.ToModel<DeviceModel>();
                    devicesModel.SelectedStoreId = device.StoreId;
                    devicesModel.StoreName = device.Store != null ? device.Store.P_BranchNo + " - " + device.Store.P_Name : string.Empty;

                    return devicesModel;
                }),
                Total = devices.TotalCount
            };

            // sort
            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            // filter
            if (searchModel.Filter?.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        #endregion
    }
}