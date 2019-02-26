using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.DataStructures;
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
    public class GetDevices_PageParameter
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

            _devices = new List<Device>();

            for (int i = 0; i < 1000; i++)
            {
                _devices.Add(new Device()
                {
                    Id = i + 1
                });
            }

            _devices = _devices.OrderBy(x => x.Id).ToList();

            var mockDevice = _devices.AsQueryable().BuildMockDbSet();
            _deviceRepository.Setup(x => x.Table).Returns(mockDevice.Object);

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Device, DeviceDto>();
        }

        [Test]
        public void Should_return_part_of_collection_corresponds_to_page_when_given_limited_devices_collection()
        {
            //Arrange
            var limit = 5;
            var page = 6;
            var expectedCollection = new ApiList<Device>(_devices.AsQueryable(), page - 1, limit);

            //Act
            var result = _deviceApiService.GetDevices(limit: limit, page: page);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            result.Count.ShouldEqual(expectedCollection.Count);
            Assert.IsTrue(result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void Should_return_first_page_when_called_with_zero_page_parameter()
        {
            //Arrange
            var limit = 5;
            var page = 0;
            var expectedCollection = new ApiList<Device>(_devices.AsQueryable(), page - 1, limit);

            //Act
            var result = _deviceApiService.GetDevices(limit: limit, page: page);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            result.Count.ShouldEqual(expectedCollection.Count);
            result.First().Id.ShouldEqual(_devices.First().Id);
            result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)).ShouldBeTrue();
        }

        [Test]
        public void Should_return_first_page_when_called_with_negative_page_parameter()
        {
            //Arrange
            var limit = 5;
            var page = -30;
            var expectedCollection = new ApiList<Device>(_devices.AsQueryable(), page - 1, limit);

            //Act
            var result = _deviceApiService.GetDevices(limit: limit, page: page);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            result.Count.ShouldEqual(expectedCollection.Count);
            result.First().Id.ShouldEqual(_devices.First().Id);
            result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)).ShouldBeTrue();
        }

        [Test]
        public void Should_return_empty_collection_when_called_with_too_big_page_parameter()
        {
            //Arrange
            var limit = 5;
            var page = _devices.Count / limit + 100;

            //Act
            var result = _deviceApiService.GetDevices(limit: limit, page: page);

            // Assert
            CollectionAssert.IsEmpty(result);
        }
    }
}