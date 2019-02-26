using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
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
using Tests;

namespace Api.Tests.ControllersTests.Devices
{
    [TestFixture]
    public class GetDevicesCount
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
        }

        [Test]
        public void Should_return_ok_result_with_count_equals_to_zero_when_no_devices_exist()
        {
            var parameters = new DevicesParametersModel();

            //Arrange
            _deviceApiService.Setup(x => x.GetDevicesCount(null, null, String.Empty)).Returns(0);

            //Act
            var result = _devicesController.GetDevicesCount(parameters).GetAwaiter().GetResult();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);
            
            Assert.IsInstanceOf<DevicesCountRootObject>(okObjectResult.Value);

            var rootObject = okObjectResult.Value as DevicesCountRootObject;
            Assert.NotNull(rootObject);

            rootObject.Count.ShouldEqual(0);
        }

        [Test]
        public void Should_return_ok_with_count_equal_to_one_when_single_device_exists()
        {
            var parameters = new DevicesParametersModel();

            //Arrange
            _deviceApiService.Setup(x => x.GetDevicesCount(null, null, String.Empty)).Returns(1);

            //Act
            var result = _devicesController.GetDevicesCount(parameters).GetAwaiter().GetResult();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            Assert.IsInstanceOf<DevicesCountRootObject>(okObjectResult.Value);

            var rootObject = okObjectResult.Value as DevicesCountRootObject;
            Assert.NotNull(rootObject);

            rootObject.Count.ShouldEqual(1);
        }

        [Test]
        public void Should_return_ok_with_count_equal_to_same_number_of_devices_when_certain_number_of_device_exist()
        {
            var parameters = new DevicesParametersModel();
            var devicesCount = 20;

            //Arrange 
            _deviceApiService.Setup(x => x.GetDevicesCount(null, null, String.Empty)).Returns(devicesCount);

            //Act
            var result = _devicesController.GetDevicesCount(parameters).GetAwaiter().GetResult();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            Assert.IsInstanceOf<DevicesCountRootObject>(okObjectResult.Value);

            var rootObject = okObjectResult.Value as DevicesCountRootObject;
            Assert.NotNull(rootObject);

            rootObject.Count.ShouldEqual(devicesCount);
        }
    }
}