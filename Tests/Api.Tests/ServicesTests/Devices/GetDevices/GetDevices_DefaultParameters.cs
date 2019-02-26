using System;
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
    public class GetDevices_DefaultParameters
    {
        private Mock<ITenantMappingService> _tenantMappingService;
        private Mock<IRepository<Device>> _deviceRepository;
        private DeviceApiService _deviceApiService;
        private DateTime _baseDate;
        private IList<Device> _devices;

        [SetUp]
        public void SetUp()
        {
            _baseDate = new DateTime(2018, 6, 6);

            _tenantMappingService = new Mock<ITenantMappingService>();
            _deviceRepository = new Mock<IRepository<Device>>();

            var device1 = new Device()
            {
                Id = 6,
                SerialNo = "SE1",
                ModelNo = "MD1",
                Longitude = 1.333,
                Latitude = 12.1331,
                StoreId = 2,
                Status = "1",
                CreatedOnUtc = _baseDate.AddDays(10),
            };

            var device2 = new Device()
            {
                Id = 2,
                SerialNo = "SE2",
                ModelNo = "MD2",
                Longitude = 18.8912,
                Latitude = 111.3123,
                StoreId = 2,
                Status = "1",
                CreatedOnUtc = _baseDate.AddMonths(1),
            };

            _devices = new List<Device> { device1, device2 };

            var mockDevice = _devices.AsQueryable().BuildMockDbSet();
            _deviceRepository.Setup(x => x.Table).Returns(mockDevice.Object);

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Device, DeviceDto>();
        }

        [Test]
        public void Should_return_empty_collection_when_no_given_devices_exist()
        {
            //Arrange
            _deviceRepository.Setup(x => x.Table).Returns(new List<Device>().AsQueryable);

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            //Act
            var result = _deviceApiService.GetDevices();

            //Assert
            result.Count.ShouldEqual(0);
        }

        [Test]
        public void Should_return_them_with_sorted_id_when_given_devices_exist()
        {
            //Act
            var result = _deviceApiService.GetDevices();

            //Assert
            result.ShouldNotBeNull();
            result.Count.ShouldEqual(2);
            result[0].SerialNo.ShouldEqual("SE2");
            result[1].ModelNo.ShouldEqual("MD1");
            result[0].Id.ShouldEqual(2);
        }
    }
}