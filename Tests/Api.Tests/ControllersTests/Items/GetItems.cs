using System;
using System.Collections.Generic;
using System.Net;
using Api.Tests.Helpers;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Controllers;
using StockManagementSystem.Api.DTOs.Items;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.Models.ItemsParameters;
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
    public class GetItems
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
        public void Should_call_service_with_same_parameters_when_valid_parameters_passed()
        {
            //Arrange
            var parameters = new ItemsParametersModel
            {
                SinceId = Configurations.DefaultSinceId + 1, // some different than default since id
                CreatedAtMin = DateTime.Now,
                CreatedAtMax = DateTime.Now,
                Page = Configurations.DefaultPageValue + 1, // some different than default page
                Limit = Configurations.MinLimit + 1 // some different than default limit
            };

            _itemApiService.Setup(x => x.GetItems(null, null, parameters.CreatedAtMin, parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId)).Returns(new List<Item>());

            //Act
            _itemsController.GetItems(parameters).GetAwaiter().GetResult();

            //Assert
            _itemApiService.Verify(x => x.GetItems(null, null, parameters.CreatedAtMin, parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId));
        }

        [Test]
        public void Should_call_serializer_with_these_items_when_some_items_exist()
        {
            //Arrange
            var parameters = new ItemsParametersModel();
            var returnedItemsCollection = new List<Item>
            {
                new Item(),
                new Item(),
            };

            _itemApiService.Setup(x => x.GetItems(parameters.GroupId, parameters.VendorId, parameters.CreatedAtMin, parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId)).Returns(returnedItemsCollection);

            //Act
            _itemsController.GetItems(parameters).GetAwaiter().GetResult();

            //Assert
            _jsonFieldsSerializer.Verify(x => x.Serialize(It.Is<ItemsRootObject>(i => i.Items.Count == returnedItemsCollection.Count), 
                It.IsIn(parameters.Fields)));
        }

        [Test]
        public void Should_call_serializer_with_no_items_when_no_items_exist()
        {
            //Arrange
            var parameters = new ItemsParametersModel();
            var returnedItemsCollection = new List<Item>();

            _itemApiService.Setup(x => x.GetItems(parameters.GroupId, parameters.VendorId, parameters.CreatedAtMin, parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId)).Returns(returnedItemsCollection);

            //Act
            _itemsController.GetItems(parameters).GetAwaiter().GetResult();

            //Assert
            _jsonFieldsSerializer.Verify(x => x.Serialize(It.Is<ItemsRootObject>(i => i.Items.Count == returnedItemsCollection.Count),
                It.IsIn(parameters.Fields)));
        }

        [Test]
        public void Should_call_serializer_with_same_fields_when_fields_passed()
        {
            //Arrange
            var parameters = new ItemsParametersModel {Fields = "stock_code,price_1,price_2,price_3,stock_type"};
            var returnedItemsCollection = new List<Item>();

            _itemApiService.Setup(x => x.GetItems(parameters.GroupId, parameters.VendorId, parameters.CreatedAtMin, parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId)).Returns(returnedItemsCollection);

            //Act
            _itemsController.GetItems(parameters).GetAwaiter().GetResult();

            //Assert
            _jsonFieldsSerializer.Verify(x => x.Serialize(It.IsAny<ItemsRootObject>(), It.IsIn(parameters.Fields)));
        }

        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void Should_return_bad_request_when_invalid_limit_passed(int invalidLimit)
        {
            //Arrange
            var parameters = new ItemsParametersModel {Limit = invalidLimit};

            var returnedItemsCollection = new List<Item>();

            _itemApiService.Setup(x => x.GetItems(parameters.GroupId, parameters.VendorId, parameters.CreatedAtMin, parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId)).Returns(returnedItemsCollection);

            //Act
            var result = _itemsController.GetItems(parameters).GetAwaiter().GetResult();
            
            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);
            statusCode.ShouldEqual(HttpStatusCode.BadRequest);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void Should_return_bad_request_when_invalid_page_passed(int invalidPage)
        {
            //Arrange
            var parameters = new ItemsParametersModel { Page = invalidPage };

            var returnedItemsCollection = new List<Item>();

            _itemApiService.Setup(x => x.GetItems(parameters.GroupId, parameters.VendorId, parameters.CreatedAtMin, parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId)).Returns(returnedItemsCollection);

            //Act
            var result = _itemsController.GetItems(parameters).GetAwaiter().GetResult();

            //Assert
            var statusCode = ActionResultExecutor.ExecuteResult(result);
            statusCode.ShouldEqual(HttpStatusCode.BadRequest);
        }
    }
}