using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Delta;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.ModelBinders;
using StockManagementSystem.Api.Models.DevicesParameters;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    public class DevicesController : BaseApiController
    {
        private readonly IFactory<Device> _factory;
        private readonly IDtoHelper _dtoHelper;
        private readonly IDeviceService _deviceService;
        private readonly IDeviceApiService _deviceApiService;

        public DevicesController(
            IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService, 
            IUserService userService, 
            ITenantMappingService tenantMappingService, 
            ITenantService tenantService, 
            IUserActivityService userActivityService, 
            IFactory<Device> factory,
            IDtoHelper dtoHelper,
            IDeviceService deviceService,
            IDeviceApiService deviceApiService) 
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _factory = factory;
            _dtoHelper = dtoHelper;
            _deviceService = deviceService;
            _deviceApiService = deviceApiService;
        }

        #region Private methods

        protected async Task<IActionResult> CountRootObjectResult(int count)
        {
            var countRootObject = new DevicesCountRootObject { Count = count > 0 ? count : 0 };

            return await Task.FromResult<IActionResult>(Ok(countRootObject));
        }

        protected async Task<IActionResult> RootObjectResult(IList<DeviceDto> entities, string fields)
        {
            var rootObj = new DevicesRootObject { Devices = entities };

            var json = JsonFieldsSerializer.Serialize(rootObj, fields);

            return await Task.FromResult<IActionResult>(new RawJsonActionResult(json));
        }

        #endregion

        /// <summary>
        /// Receive a list of all devices
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/devices")]
        [ProducesResponseType(typeof(DevicesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetDevices(DevicesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var devices = _deviceApiService.GetDevices(parameters.Ids, parameters.Limit, parameters.Page,
                    parameters.SinceId, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status)
                .Where(d => TenantMappingService.Authorize(d));

            IList<DeviceDto> devicesAsDtos = devices.Select(device => _dtoHelper.PrepareDeviceDto(device)).ToList();

            var devicesRootObject = new DevicesRootObject {Devices = devicesAsDtos};

            var json = JsonFieldsSerializer.Serialize(devicesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all devices
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/devices/count")]
        [ProducesResponseType(typeof(DevicesCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetDevicesCount(DevicesParametersModel parameters)
        {
            var devicesCount = _deviceApiService.GetDevicesCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status);

            var devicesCountRootObject = new DevicesCountRootObject()
            {
                Count = devicesCount
            };

            return await Task.FromResult<IActionResult>(Ok(devicesCountRootObject));
        }

        /// <summary>
        /// Retrieve device by id
        /// </summary>
        /// <param name="id">Id of the device</param>
        /// <param name="fields">Fields from the device you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/devices/{id}")]
        [ProducesResponseType(typeof(DevicesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetDeviceById(int id, string fields = "")
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var device = _deviceApiService.GetDeviceById(id);
            if (device == null)
                return await Error(HttpStatusCode.NotFound, "device", "not found");

            var deviceDto = _dtoHelper.PrepareDeviceDto(device);

            var devicesRootObject = new DevicesRootObject();
            devicesRootObject.Devices.Add(deviceDto);

            var json = JsonFieldsSerializer.Serialize(devicesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Search for related device matching supplied query
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/devices/search")]
        [ProducesResponseType(typeof(DevicesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Search(GenericSearchParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            var entities = _deviceApiService.Search(
                parameters.Query,
                parameters.Limit,
                parameters.Page,
                parameters.SortColumn,
                parameters.Descending,
                parameters.Count);

            return parameters.Count
                ? await CountRootObjectResult(entities.Count)
                : await RootObjectResult(entities.List, parameters.Fields);
        }

        /// <summary>
        /// Create new device
        /// </summary>
        [HttpPost]
        [Route("/api/devices")]
        [ProducesResponseType(typeof(DevicesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateDevice([ModelBinder(typeof(JsonModelBinder<DeviceDto>))] Delta<DeviceDto> deviceDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var newDevice = _factory.Initialize();
            deviceDelta.Merge(newDevice);

            await _deviceService.InsertDevice(newDevice);

            await UpdateTenantMappings(newDevice, deviceDelta.Dto.TenantIds);

            //activity log
            await UserActivityService.InsertActivityAsync("AddNewDevice", $"Added a new device (ID = {newDevice.Id})", newDevice);

            var newDeviceDto = _dtoHelper.PrepareDeviceDto(newDevice);

            var devicesRootObject = new DevicesRootObject();
            devicesRootObject.Devices.Add(newDeviceDto);

            var json = JsonFieldsSerializer.Serialize(devicesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Update device by attributes
        /// </summary>
        /// <param name="id">Device id on <see cref="NameValueCollection"/>format</param>
        /// <param name="serialno">Device serial no <see cref="NameValueCollection"/>format</param>
        [HttpPut]
        [Route("/api/devices")]
        [ProducesResponseType(typeof(DevicesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateDeviceByAttributes([FromQuery] int id, [FromQuery] string serialno, [ModelBinder(typeof(JsonModelBinder<DeviceDto>))] Delta<DeviceDto> deviceDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            Device currentDevice;

            if (id > 0)
                currentDevice = _deviceApiService.GetDeviceById(id);
            else if (!string.IsNullOrEmpty(serialno))
                currentDevice = _deviceApiService.GetDeviceBySerialNo(serialno);
            else
                return await Error(HttpStatusCode.BadRequest, "invalid", "invalid id or serial_no");

            if (currentDevice == null)
                return await Error(HttpStatusCode.NotFound, "device", "not found");

            deviceDelta.Merge(currentDevice);
            currentDevice.ModifiedOnUtc = DateTime.UtcNow;

            _deviceService.UpdateDevice(currentDevice);

            await UpdateTenantMappings(currentDevice, deviceDelta.Dto.TenantIds);
            await UserActivityService.InsertActivityAsync("EditDevice",
                $"Edited a device (ID = {currentDevice.Id}, SN = {currentDevice.SerialNo})", currentDevice);

            var deviceDto = _dtoHelper.PrepareDeviceDto(currentDevice);

            var devicesRootObject = new DevicesRootObject();
            devicesRootObject.Devices.Add(deviceDto);

            var json = JsonFieldsSerializer.Serialize(devicesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Delete device by id
        /// </summary>
        [HttpDelete]
        [Route("/api/devices/{id}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "Invalid id");

            var deviceToDelete = _deviceApiService.GetDeviceById(id);
            if (deviceToDelete == null)
                return await Error(HttpStatusCode.NotFound, "device", "Device not found");

            _deviceService.DeleteDevice(deviceToDelete);

            //activity log
            await UserActivityService.InsertActivityAsync("DeleteDevice", $"Deleted a device (ID = {id})", deviceToDelete);

            return new RawJsonActionResult("{}");
        }
    }
}