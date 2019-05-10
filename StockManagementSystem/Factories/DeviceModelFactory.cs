using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Devices;
using StockManagementSystem.Models.Setting;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Extensions;
using StockManagementSystem.Web.Kendoui.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Web.Factories;

namespace StockManagementSystem.Factories
{
    /// <summary>
    /// Represents the device model factory implementation
    /// </summary>
    public class DeviceModelFactory : IDeviceModelFactory
    {
        private readonly IBaseModelFactory _baseModelFactory;
        private readonly IDeviceService _deviceService;
        private readonly IStoreService _storeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IConfiguration _configuration;

        public DeviceModelFactory(
            IBaseModelFactory baseModelFactory,
            IDeviceService deviceService,
            IStoreService storeService,
            IDateTimeHelper dateTimeHelper,
            IConfiguration configuration)
        {
            _baseModelFactory = baseModelFactory;
            _deviceService = deviceService;
            _storeService = storeService;
            _dateTimeHelper = dateTimeHelper;
            _configuration = configuration;
        }

        #region Manage Device

        public async Task<DeviceSearchModel> PrepareDeviceSearchModel(DeviceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            await _baseModelFactory.PrepareStores(searchModel.AvailableStores);

            searchModel.SetGridPageSize();

            return searchModel;
        }        

        public async Task<DeviceListModel> PrepareDeviceListModel(DeviceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var devices = await _deviceService.GetDevicesAsync( 
                storeIds: searchModel.SelectedStoreIds.ToArray(),
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
                    devicesModel.StoreName = device.Store.P_BranchNo + " - " + device.Store.P_Name;
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
            if (searchModel.Filter?.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }

        public async Task<DeviceModel> PrepareDeviceListbyStoreModel(int storeID)
        {
            var devices = await _deviceService.GetDevicesByStoreIdAsync(storeID);

            var model = new DeviceModel
            {
                Devices = devices
            };

            return model;
        }

        public async Task<DeviceModel> PrepareDeviceListModel()
        {
            var devices = await _deviceService.GetAllDevicesAsync();

            var model = new DeviceModel
            {
                Devices = devices
            };

            foreach (var item in model.Devices)
            {
                double distance = getDistance(item.Latitude, item.Longitude, (double)item.Store.Latitude, (double)item.Store.Longitude) / 1000; //returns in KM
                if (distance > Convert.ToDouble(_configuration["OutofRadarRadius"]))
                {
                    item.Status = "2";
                }
            }

            return model;
        }

        private double getDistance(double latitude, double longitude, double otherLatitude, double otherLongitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        public async Task<DeviceModel> PrepareDeviceModel(DeviceModel model, Device device)
        {
            if (device != null)
            {
                model = model ?? new DeviceModel();

                model.Id = device.Id;
                model.SerialNo = device.SerialNo;
                model.ModelNo = device.ModelNo;
                model.StoreName = device.Store.P_BranchNo + " - " + device.Store.P_Name;
                model.SelectedStoreId = device.StoreId;
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(device.CreatedOnUtc, DateTimeKind.Utc);
                model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(device.ModifiedOnUtc.GetValueOrDefault(DateTime.UtcNow), DateTimeKind.Utc);
                model.SelectedStoreId = device.Store.Id;
            }

            var stores = await _storeService.GetStores();
            model.AvailableStores = stores.Select(store => new SelectListItem
            {
                Text = store.P_BranchNo.ToString() + " - " + store.P_Name,
                Value = store.P_BranchNo.ToString()
            }).ToList();

            return await Task.FromResult(model);
        }

        #endregion

        #region Device Tracking

        public async Task<DeviceTrackingContainerModel> PrepareDeviceTrackingContainerModel(
            DeviceTrackingContainerModel deviceTrackingContainerModel)
        {
            if (deviceTrackingContainerModel == null)
                throw new ArgumentNullException(nameof(deviceTrackingContainerModel));

            //prepare nested models
            await PrepareMapDeviceListingModel(deviceTrackingContainerModel.DeviceListing);
            await PrepareMapDeviceModel();

            return deviceTrackingContainerModel;
        }

        public async Task<MapDeviceListModel> PrepareMapDeviceListingModel(MapDeviceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var mapList = await _deviceService.GetAllDevicesAsync();

            if (mapList == null)
                throw new ArgumentNullException(nameof(mapList));

            var model = new MapDeviceListModel
            {
                Data = mapList.PaginationByRequestModel(searchModel).Select(mapLst =>
                {
                    var mapListModel = mapLst.ToModel<MapDeviceModel>();
                    mapListModel.StoreName = mapLst.Store.P_BranchNo + " - " + mapLst.Store.P_Name;

                    double distance = getDistance(mapLst.Latitude, mapLst.Longitude, (double)mapLst.Store.Latitude, (double)mapLst.Store.Longitude) / 1000; //returns in KM
                    if (distance > Convert.ToDouble(_configuration["OutofRadarRadius"]))
                    {
                        mapLst.Status = "2";
                    }
                    mapListModel.Status = (mapLst.Status == null || mapLst.Status == "0" ) ? "Offline" : mapLst.Status == "1" ? "Online" : mapLst.Status == "2" ? "Out of radar" : "N/A";
                    return mapListModel;
                }),
                Total = mapList.Count
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

        public async Task<DeviceModel> PrepareMapDeviceModel()
        {
            var devices = await _deviceService.GetAllDevicesAsync();

            var model = new DeviceModel
            {
                Devices = devices
            };

            return model;
        }

        #endregion
    }
}