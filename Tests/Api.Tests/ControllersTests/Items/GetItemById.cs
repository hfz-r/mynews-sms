using System.Net;
using Api.Tests.Helpers;
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
using Tests;

namespace Api.Tests.ControllersTests.Items
{
    [TestFixture]
    public class GetItemById
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
        [TestCase(0)]
        [TestCase(-20)]
        public void Should_return_bad_request_when_id_equals_to_zero_or_less(int nonPositiveDeviceId)
        {
            //Arrange
            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<ItemsRootObject>(), It.IsAny<string>()))
                .Returns(string.Empty);

            //Act
            var result = _itemsController.GetItemById(nonPositiveDeviceId).GetAwaiter().GetResult();

            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);
            statusCode.ShouldEqual(HttpStatusCode.BadRequest);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void Should_not_call_ItemApiService_when_id_equals_to_zero_or_less(int negativeDeviceId)
        {
            //Arrange
            _jsonFieldsSerializer.Setup(x => x.Serialize(null, null)).Returns(string.Empty);

            //Act
            var result = _itemsController.GetItemById(negativeDeviceId).GetAwaiter().GetResult();

            //Assert
            _itemApiService.Verify(x => x.GetItemById(negativeDeviceId), Times.Never);
        }

        [Test]
        public void Should_return_404_not_found_when_id_is_positive_but_not_exist_in_item()
        {
            var nonExistingItemId = 5;

            //Arrange
            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<ItemsRootObject>(), It.IsAny<string>()))
                .Returns(string.Empty);

            _itemApiService.Setup(x => x.GetItemById(nonExistingItemId)).Returns(() => null);

            //Act
            var result = _itemsController.GetItemById(nonExistingItemId).GetAwaiter().GetResult();

            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);
            statusCode.ShouldEqual(HttpStatusCode.NotFound);
        }
    }
}