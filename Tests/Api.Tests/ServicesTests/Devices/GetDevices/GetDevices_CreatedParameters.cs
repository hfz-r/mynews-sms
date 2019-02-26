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
    public class GetDevices_CreatedParameters
    {
        private Mock<IRepository<Device>> _deviceRepository;
        private Mock<ITenantMappingService> _tenantMappingService;
        private DeviceApiService _deviceApiService;
        private DateTime _baseDate;
        private IList<Device> _devices;

        [SetUp]
        public void SetUp()
        {
            _baseDate = new DateTime(2016, 2, 23);

            _tenantMappingService = new Mock<ITenantMappingService>();
            _deviceRepository = new Mock<IRepository<Device>>();

            _devices = new List<Device>
            {
                new Device() {Id =2, CreatedOnUtc = _baseDate.AddMonths(2) },
                new Device() {Id =3, CreatedOnUtc = _baseDate.AddMonths(10) },
                new Device() {Id =1,  CreatedOnUtc = _baseDate.AddMonths(7) },
                new Device() {Id =4, CreatedOnUtc = _baseDate},
                new Device() {Id =5, CreatedOnUtc = _baseDate.AddMonths(3)},
            };

            var mockDevice = _devices.AsQueryable().BuildMockDbSet();
            _deviceRepository.Setup(x => x.Table).Returns(mockDevice.Object);

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Device, DeviceDto>();
        }

        [Test]
        public void Should_return_devices_sorted_by_id_when_devices_created_after_given_date()
        {
            //Arrange
            DateTime createdAtMinDate = _baseDate.AddMonths(5);
            var expectedCollection = _devices.Where(x => x.CreatedOnUtc > createdAtMinDate).OrderBy(x => x.Id);

            //Act
            var result = _deviceApiService.GetDevices(createdAtMin: createdAtMinDate);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            result.Count.ShouldEqual(expectedCollection.Count());
            Assert.IsTrue(result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void Should_return_empty_collection_when_given_devices_created_before_that_date()
        {
            //Arrange
            DateTime createdAtMinDate = _baseDate.AddMonths(11);

            //Act
            var result = _deviceApiService.GetDevices(createdAtMin: createdAtMinDate);

            //Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Should_return_devices_sorted_by_id_when_devices_created_before_that_date()
        {
            //Arrange
            DateTime createdAtMaxDate = _baseDate.AddMonths(5);
            var expectedCollection = _devices.Where(x => x.CreatedOnUtc < createdAtMaxDate).OrderBy(x => x.Id);

            //Act
            var result = _deviceApiService.GetDevices(createdAtMax: createdAtMaxDate);

            //Assert
            CollectionAssert.IsNotEmpty(result);
            result.Count.ShouldEqual(expectedCollection.Count());
            Assert.IsTrue(result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)));
        }

        [Test]
        public void Should_return_empty_collection_when_given_devices_created_after_that_date()
        {
            //Arrange
            DateTime createdAtMaxDate = _baseDate.Subtract(new TimeSpan(365)); // subtract one year

            //Act
            var result = _deviceApiService.GetDevices(createdAtMax: createdAtMaxDate);

            // Assert
            CollectionAssert.IsEmpty(result);
        }
    }
}