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
    public class GetDevices_LimitParameter
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

            var mockDevice = _devices.AsQueryable().BuildMockDbSet();
            _deviceRepository.Setup(x => x.Table).Returns(mockDevice.Object);

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Device, DeviceDto>();
        }

        [Test]
        public void Should_return_collection_with_count_equals_to_the_given_devices_a_limit()
        {
            //Arrange
            var expectedLimit = 5;

            //Act
            var result = _deviceApiService.GetDevices(limit: expectedLimit);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            result.Count.ShouldEqual(expectedLimit);
        }

        [Test]
        public void Should_return_empty_collection_when_given_limit_equals_zero_or_negative()
        {
            //Arrange 
            var expectedLimitZero = 0;
            var expectedLimitNegative = -10;

            //Act
            var resultZero = _deviceApiService.GetDevices(limit: expectedLimitZero);
            var resultNegative = _deviceApiService.GetDevices(limit: expectedLimitNegative);

            //Assert
            CollectionAssert.IsEmpty(resultZero);
            CollectionAssert.IsEmpty(resultNegative);
        }
    }
}