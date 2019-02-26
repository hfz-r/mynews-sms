using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.Infrastructure.Mapper;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Services.Tenants;
using Tests;

namespace Api.Tests.ServicesTests.Devices.GetDevices
{
    [TestFixture]
    public class GetDevices_SinceIdParameter
    {
        private Mock<ITenantMappingService> _tenantMappingService;
        private Mock<IRepository<Device>> _deviceRepository;
        private DeviceApiService _deviceApiService;
        private IList<Device> _devices;

        [SetUp]
        public void SetUp()
        {
            _tenantMappingService = new Mock<ITenantMappingService>();
            _deviceRepository = new Mock<IRepository<Device>>();

            _devices = new List<Device>
            {
                new Device() {Id =2},
                new Device() {Id =3},
                new Device() {Id =1},
                new Device() {Id =4},
                new Device() {Id =5},
            };

            var mockDevice = _devices.AsQueryable().BuildMockDbSet();
            _deviceRepository.Setup(x => x.Table).Returns(mockDevice.Object);

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Device, DeviceDto>();
        }

        [Test]
        public void Should_return_only_devices_after_id_is_sorted_when_called_with_valid_since_id()
        {
            //Arrange
            int sinceId = 3;
            var expectedCollection = _devices.Where(x => x.Id > sinceId).OrderBy(x => x.Id);

            //Act
            var result = _deviceApiService.GetDevices(sinceId: sinceId);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            Assert.IsTrue(result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void Should_return_all_devices_sorted_by_id_when_called_zero_or_negative_since_id(int sinceId)
        {
            //Arrange
            var expectedCollection = _devices.Where(x => x.Id > sinceId).OrderBy(x => x.Id);

            //Act
            var result = _deviceApiService.GetDevices(sinceId: sinceId);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            Assert.IsTrue(result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void Should_return_empty_collection_when_called_since_id_outside_id_range()
        {
            //Arrange
            int sinceId = int.MaxValue;

            //Act
            var result = _deviceApiService.GetDevices(sinceId: sinceId);

            // Assert
            CollectionAssert.IsEmpty(result);
        }
    }
}