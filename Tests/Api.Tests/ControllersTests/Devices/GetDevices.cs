using System;
using System.Collections.Generic;
using System.Net;
using Api.Tests.Helpers;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Controllers;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.Models.DevicesParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace Api.Tests.ControllersTests.Devices
{
    [TestFixture]
    public class GetDevices
    {
        private Mock<IJsonFieldsSerializer> _jsonFieldsSerializer;
        private Mock<IAclService> _aclService;
        private Mock<IUserService> _userService;
        private Mock<ITenantMappingService> _tenantMappingService;
        private Mock<ITenantService> _tenantService;
        private Mock<IUserActivityService> _userActivityService;
        private Mock<IFactory<Device>> _factoryDevice;
        private Mock<IDtoHelper> _dtoHelper;
        private Mock<IDeviceService> _deviceService;
        private Mock<IDeviceApiService> _deviceApiService;

        private DevicesController _devicesController;

        [SetUp]
        public void SetUp()
        {
            _jsonFieldsSerializer = new Mock<IJsonFieldsSerializer>();
            _aclService = new Mock<IAclService>();
            _userService = new Mock<IUserService>();
            _tenantMappingService = new Mock<ITenantMappingService>();
            _tenantService = new Mock<ITenantService>();
            _userActivityService = new Mock<IUserActivityService>();
            _factoryDevice = new Mock<IFactory<Device>>();
            _dtoHelper = new Mock<IDtoHelper>();
            _deviceService = new Mock<IDeviceService>();
            _deviceApiService = new Mock<IDeviceApiService>();

            _devicesController = new DevicesController(
                _jsonFieldsSerializer.Object,
                _aclService.Object,
                _userService.Object,
                _tenantMappingService.Object,
                _tenantService.Object,
                _userActivityService.Object,
                _factoryDevice.Object,
                _dtoHelper.Object,
                _deviceService.Object,
                _deviceApiService.Object);

            _tenantMappingService.Setup(x => x.Authorize(It.IsAny<Device>())).Returns(true);
        }

        [Test]
        public void Should_call_service_with_same_parameters_when_valid_parameters_passed()
        {
            //Arrange
            var returnedDevicesList = new List<Device>();
            var parameters = new DevicesParametersModel
            {
                SinceId = Configurations.DefaultSinceId + 1, // some different than default since id
                CreatedAtMin = DateTime.Now,
                CreatedAtMax = DateTime.Now,
                Page = Configurations.DefaultPageValue + 1, // some different than default page
                Limit = Configurations.MinLimit + 1, // some different than default limit
            };

            _deviceApiService.Setup(x => x.GetDevices(null, parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.CreatedAtMin, parameters.CreatedAtMax, string.Empty)).Returns(returnedDevicesList);

            //Act
            _devicesController.GetDevices(parameters).GetAwaiter().GetResult();

            //Assert
            _deviceApiService.Verify(x => x.GetDevices(null, parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.CreatedAtMin, parameters.CreatedAtMax, string.Empty));
        }

        [Test]
        public void Should_call_serializer_with_these_devices_when_some_devices_exist()
        {
            //Arrange
            var returnedDevicesList = new List<Device>
            {
                new Device(),
                new Device(),
            };
            var parameters = new DevicesParametersModel();

            _deviceApiService.Setup(x => x.GetDevices(parameters.Ids, parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status)).Returns(returnedDevicesList);

            //Act
            _devicesController.GetDevices(parameters).GetAwaiter().GetResult();

            //Assert
            _jsonFieldsSerializer.Verify(x => x.Serialize(It.Is<DevicesRootObject>(d => d.Devices.Count == returnedDevicesList.Count), 
                It.IsIn(parameters.Fields)));
        }

        [Test]
        public void Should_call_serializer_with_no_devices_when_no_devices_exist()
        {
            //Arrange
            var returnedDevicesList = new List<Device>();
            var parameters = new DevicesParametersModel();

            _deviceApiService.Setup(x => x.GetDevices(parameters.Ids, parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status)).Returns(returnedDevicesList);

            //Act
            _devicesController.GetDevices(parameters).GetAwaiter().GetResult();

            //Assert
            _jsonFieldsSerializer.Verify(x => x.Serialize(It.Is<DevicesRootObject>(d => d.Devices.Count == returnedDevicesList.Count), 
                It.IsIn(parameters.Fields)));
        }

        [Test]
        public void Should_call_serializer_with_same_fields_when_fields_passed()
        {
            //Arrange
            var returnedDevicesList = new List<Device>();
            var parameters = new DevicesParametersModel
            {
                Fields = "id,email"
            };

            _deviceApiService.Setup(x => x.GetDevices(parameters.Ids, parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status)).Returns(returnedDevicesList);

            //Act
            _devicesController.GetDevices(parameters).GetAwaiter().GetResult();

            //Assert
            _jsonFieldsSerializer.Verify(x => x.Serialize(It.IsAny<DevicesRootObject>(), It.IsIn(parameters.Fields)));
        }

        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void Should_return_bad_request_when_invalid_limit_passed(int invalidLimit)
        {
            //Arrange
            var returnedDevicesList = new List<Device>();
            var parameters = new DevicesParametersModel
            {
                Limit = invalidLimit
            };

            _deviceApiService.Setup(x => x.GetDevices(parameters.Ids, parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status)).Returns(returnedDevicesList);

            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<DevicesRootObject>(), It.IsAny<string>()))
                .Returns(String.Empty);

            //Act
            var result = _devicesController.GetDevices(parameters).GetAwaiter().GetResult();
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void Should_return_bad_request_when_invalid_page_passed(int invalidPage)
        {
            //Arrange
            var returnedDevicesList = new List<Device>();
            var parameters = new DevicesParametersModel
            {
                Page = invalidPage
            };

            _deviceApiService.Setup(x => x.GetDevices(parameters.Ids, parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status)).Returns(returnedDevicesList);

            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<DevicesRootObject>(), It.IsAny<string>()))
                .Returns(String.Empty);

            //Act
            var result = _devicesController.GetDevices(parameters).GetAwaiter().GetResult();
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }
    }
}