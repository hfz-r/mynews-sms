using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Services.Tenants;
using Tests;

namespace Api.Tests.ServicesTests.Devices.GetDeviceById
{
    [TestFixture]
    public class GetDeviceById
    {
        private Mock<IRepository<Device>> _deviceRepository;
        private Mock<ITenantMappingService> _tenantMappingService;
        private DeviceApiService _deviceApiService;

        [SetUp]
        public void SetUp()
        {
            _deviceRepository = new Mock<IRepository<Device>>();
            _tenantMappingService = new Mock<ITenantMappingService>();
        }

        [Test]
        public void Should_return_null_when_null_is_returned_by_repository()
        {
            int deviceId = 3;

            //Arrange
            _deviceRepository.Setup(x => x.Table).Returns(new List<Device>().AsQueryable());
            _deviceRepository.Setup(x => x.GetById(deviceId)).Returns(new Device());

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            //Act
            var result = _deviceApiService.GetDeviceById(deviceId);

            //Assert
            result.ShouldBeNull();
        }

        [Test]
        [TestCase(-2)]
        [TestCase(0)]
        public void Should_return_null_when_negative_or_zero_device_id_passed(int negativeOrZeroDeviceId)
        {
            //Arrange
            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            //Act
            var result = _deviceApiService.GetDeviceById(negativeOrZeroDeviceId);

            //Assert
            result.ShouldBeNull();
        }

        [Test]
        public void Should_return_same_device_when_device_is_returned_by_repository()
        {
            int deviceId = 3;
            Device device = new Device() {Id = 3, ModelNo = "some model"};

            //Arrange
            var list = new List<Device>();
            list.Add(device);

            _deviceRepository.Setup(x => x.Table).Returns(list.AsQueryable());
            _deviceRepository.Setup(x => x.GetById(deviceId)).Returns(device);

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            //Act
            var result = _deviceApiService.GetDeviceById(deviceId);

            //Assert
            result.ShouldBeTheSameAs(device);
        }
    }
}