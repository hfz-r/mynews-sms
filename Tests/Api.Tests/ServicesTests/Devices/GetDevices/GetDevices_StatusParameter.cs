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
    public class GetDevices_StatusParameter
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
                new Device() {Id =2, Status = "1"},
                new Device() {Id =3, Status = "2"},
                new Device() {Id =1, Status = "2"},
                new Device() {Id =4, Status = "2"},
                new Device() {Id =5},
            };

            var mockDevice = _devices.AsQueryable().BuildMockDbSet();
            _deviceRepository.Setup(x => x.Table).Returns(mockDevice.Object);

            _deviceApiService = new DeviceApiService(_tenantMappingService.Object, _deviceRepository.Object);

            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Device, DeviceDto>();
        }

        [Test]
        public void Should_return_them_sorted_by_id_when_ask_for_devices_status()
        {
            //Arrange
            var expectedCollection = _devices.Where(x => x.Status == "2").OrderBy(x => x.Id);

            //Act
            var result = _deviceApiService.GetDevices(status: "2");

            //Assert
            CollectionAssert.IsNotEmpty(result);
            result.Select(x => x.Id).SequenceEqual(expectedCollection.Select(x => x.Id)).ShouldBeTrue();
        }
    }
}