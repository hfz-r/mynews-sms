using System.Net;
using Api.Tests.Helpers;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.Controllers;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.Json.Serializer;
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
    public class GetDeviceById
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
        [TestCase(0)]
        [TestCase(-20)]
        public void Should_return_bad_request_when_id_equals_to_zero_or_less(int nonPositiveDeviceId)
        {
            //Arrange
            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<DevicesRootObject>(), It.IsAny<string>(), null))
                .Returns(string.Empty);

            //Act
            var result = _devicesController.GetDeviceById(nonPositiveDeviceId).GetAwaiter().GetResult();

            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void Should_not_call_DeviceApiService_when_id_equals_to_zero_or_less(int negativeDeviceId)
        {
            //Arrange
            _jsonFieldsSerializer.Setup(x => x.Serialize(null, null, null)).Returns(string.Empty);

            //Act
            _devicesController.GetDeviceById(negativeDeviceId).GetAwaiter().GetResult();

            //Assert
            _deviceApiService.Verify(x => x.GetDeviceById(negativeDeviceId), Times.Never);
        }

        [Test]
        public void Should_return_404_not_found_when_id_is_positive_but_not_exist_in_device()
        {
            var nonExistingDeviceId = 5;

            //Arrange
            _deviceApiService.Setup(x => x.GetDeviceById(nonExistingDeviceId)).Returns(() => null);

            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<DevicesRootObject>(), It.IsAny<string>(), null))
                .Returns(string.Empty);

            //Act
            var result = _devicesController.GetDeviceById(nonExistingDeviceId).GetAwaiter().GetResult();

            // Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);
            Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        }
    }
}