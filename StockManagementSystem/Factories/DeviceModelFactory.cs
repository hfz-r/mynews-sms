using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Devices;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Kendoui.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Factories
{
    /// <summary>
    /// Represents the device model factory implementation
    /// </summary>
    public class DeviceModelFactory : IDeviceModelFactory
    {
        private readonly IDeviceService _deviceService;
        private readonly IStoreService _storeService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public DeviceModelFactory(
            IDeviceService deviceService,
            IStoreService storeService,
            IDateTimeHelper dateTimeHelper)
        {
            _deviceService = deviceService;
            _storeService = storeService;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<DeviceSearchModel> PrepareDeviceSearchModel(DeviceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            var stores = await _storeService.GetStoresAsync();
            searchModel.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            return await Task.FromResult(searchModel);
        }        

        public async Task<DeviceListModel> PrepareDeviceListModel(DeviceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var devices = await _deviceService.GetDevicesAsync(
                storeIds: searchModel.SelectedStoreId.ToArray(),
                serialNo: searchModel.SearchSerialNo,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new DeviceListModel
            {
                Data = devices.Select(device =>
                {
                    var devicesModel = device.ToModel<DeviceModel>();

                    devicesModel.SerialNo = device.SerialNo;
                    devicesModel.ModelNo = device.ModelNo;
                    devicesModel.SelectedStoreId = device.StoreId;
                    devicesModel.StoreName = device.Store.P_Name;
                    devicesModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(device.CreatedOnUtc, DateTimeKind.Utc);
                    devicesModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(device.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);

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
            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        public async Task<DeviceModel> PrepareDeviceListModel()
        {
            var devices = await _deviceService.GetAllDevicesAsync();

            var model = new DeviceModel
            {
                Devices = devices
            };            

            return model;
        }

        public async Task<DeviceModel> PrepareDeviceModel(DeviceModel model, Device device)
        {
            if (device != null)
            {
                model = model ?? new DeviceModel();

                model.Id = device.Id;
                model.SerialNo = device.SerialNo;
                model.ModelNo = device.ModelNo;
                model.StoreName = device.Store.P_Name;
                model.SelectedStoreId = device.StoreId;
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(device.CreatedOnUtc, DateTimeKind.Utc);
                model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(device.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);
                model.SelectedStoreId = device.Store.Id;
            }

            var stores = await _storeService.GetStoresAsync();
            model.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            return await Task.FromResult(model);
        }
    }
}