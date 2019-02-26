using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Services.Tenants;
using Tests;

namespace Api.Tests.ServicesTests.Devices.GetDevicesCount
{
    [TestFixture]
    public class GetDevicesCount_DefaultParameters
    {
        private Mock<ITenantMappingService> _tenantMappingService;
        private Mock<IRepository<Device>> _deviceRepository;
        private DeviceApiService _deviceApiService;

        [SetUp]
        public void SetUp()
        {
            _tenantMappingService = new Mock<ITenantMappingService>();
            _deviceRepository = new Mock<IRepository<Device>>();

            _tenantMappingService.Setup(x => x.Authorize(It.IsAny<Device>())).Returns(true);
        }

        [Test]
        public void Should_return_zero_when_given_no_devices_exist()
        {
            //Arrange
            _deviceRepository.Setup(x => x.Table).Returns(new List<Device>().AsQueryable());

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            //Act
            var devicesCount = _deviceApiService.GetDevicesCount();

            //Assert
            devicesCount.ShouldEqual(0);
        }

        [Test]
        public void Should_return_their_count_when_given_exist_devices()
        {
            var devices = new List<Device>
            {
                new Device {Id = 1},
                new Device {Id = 2},
                new Device {Id = 3},
            };

            //Arrange
            var mockDevices = devices.AsQueryable().BuildMockDbSet();
            _deviceRepository.Setup(x => x.Table).Returns(mockDevices.Object);

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            //Act
            var devicesCount = _deviceApiService.GetDevicesCount();

            //Assert
            devicesCount.ShouldEqual(3);
        }
    }
}