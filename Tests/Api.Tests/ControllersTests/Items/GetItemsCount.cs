using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.Controllers;
using StockManagementSystem.Api.DTOs.Items;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace Api.Tests.ControllersTests.Items
{
    [TestFixture]
    public class GetItemsCount
    {
        private Mock<IJsonFieldsSerializer> _jsonFieldsSerializer;
        private Mock<IAclService> _aclService;
        private Mock<IUserService> _userService;
        private Mock<ITenantMappingService> _tenantMappingService;
        private Mock<ITenantService> _tenantService;
        private Mock<IUserActivityService> _userActivityService;
        private Mock<IItemApiService> _itemApiService;
        private Mock<IRepository<Item>> _itemRepository;

        private ItemsController _itemsController;

        [SetUp]
        public void SetUp()
        {
            _jsonFieldsSerializer = new Mock<IJsonFieldsSerializer>();
            _aclService = new Mock<IAclService>();
            _userService = new Mock<IUserService>();
            _tenantMappingService = new Mock<ITenantMappingService>();
            _tenantService = new Mock<ITenantService>();
            _userActivityService = new Mock<IUserActivityService>();
            _itemApiService = new Mock<IItemApiService>();
            _itemRepository = new Mock<IRepository<Item>>();

            _itemsController = new ItemsController(
                _jsonFieldsSerializer.Object,
                _aclService.Object,
                _userService.Object,
                _tenantMappingService.Object,
                _tenantService.Object,
                _userActivityService.Object,
                _itemApiService.Object,
                _itemRepository.Object);
        }

        [Test]
        public void Should_return_ok_with_count_equals_zero_when_no_item_exists()
        {
            //Arrange
            _itemApiService.Setup(x => x.GetItemsCount(50, 1, 0)).Returns(0);

            //Act
            var result = _itemsController.GetItemsCount().GetAwaiter().GetResult();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(0, ((ItemsCountRootObject)((OkObjectResult)result).Value).Count);
        }

        [Test]
        public void Should_return_ok_with_count_equal_to_one_when_single_item_exists()
        {
            //Arrange
            _itemApiService.Setup(x => x.GetItemsCount(50, 1, 0)).Returns(1);

            //Act
            var result = _itemsController.GetItemsCount().GetAwaiter().GetResult();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(1, ((ItemsCountRootObject)((OkObjectResult)result).Value).Count);
        }

        [Test]
        public void Should_return_ok_with_count_equals_to_same_items_when_certain_number_items_exists()
        {
            var itemsCount = 20;

            //Arrange
            _itemApiService.Setup(x => x.GetItemsCount(50, 1, 0)).Returns(itemsCount);

            //Act
            var result = _itemsController.GetItemsCount().GetAwaiter().GetResult();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(itemsCount, ((ItemsCountRootObject)((OkObjectResult)result).Value).Count);
        }
    }
}