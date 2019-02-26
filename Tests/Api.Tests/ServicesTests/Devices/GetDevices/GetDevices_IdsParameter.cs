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
    public class GetDevices_IdsParameter
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
        public void Should_return_them_sorted_by_id_when_given_devices_with_specified_ids()
        {
            //Arrange
            var idsCollection = new List<int>() { 1, 5 };

            //Act
            var result = _deviceApiService.GetDevices(ids: idsCollection);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            result[0].Id.ShouldEqual(idsCollection[0]);
            result[1].Id.ShouldEqual(idsCollection[1]);
        }

        [Test]
        public void Should_return_them_sorted_by_id_when_given_devices_with_some_of_the_specified_ids()
        {
            //Arrange
            var idsCollection = new List<int>() { 1, 5, 97373, 4 };

            //Act
            var result = _deviceApiService.GetDevices(ids: idsCollection);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            result[0].Id.ShouldEqual(idsCollection[0]);
            result[1].Id.ShouldEqual(idsCollection[3]);
            result[2].Id.ShouldEqual(idsCollection[1]);
        }

        [Test]
        public void Should_return_empty_collection_when_given_devices_that_not_match_with_specified_ids()
        {
            //Arrange
            var idsCollection = new List<int>() { 2123434, 5456456, 97373, -45 };

            //Act
            var result = _deviceApiService.GetDevices(ids: idsCollection);

            //Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Should_return_them_when_given_empty_ids_collection()
        {
            //Arrange
            var idsCollection = new List<int>();

            //Act
            var result = _deviceApiService.GetDevices(ids: idsCollection);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            result.Count.ShouldEqual(_devices.Count);
        }
    }
}