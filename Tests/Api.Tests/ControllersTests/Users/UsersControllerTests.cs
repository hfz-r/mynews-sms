using System;
using System.Collections.Generic;
using System.Net;
using Api.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Controllers;
using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.Models.UsersParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace Api.Tests.ControllersTests.Users
{
    [TestFixture]
    public class UsersControllerTests
    {
        private Mock<IJsonFieldsSerializer> _jsonFieldsSerializer;
        private Mock<IAclService> _aclService;
        private Mock<IUserService> _userService;
        private Mock<ITenantMappingService> _tenantMappingService;
        private Mock<ITenantService> _tenantService;
        private Mock<IUserActivityService> _userActivityService;
        private Mock<IUserApiService> _userApiService;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Mock<IEncryptionService> _encryptionService;
        private Mock<IStoreService> _storeService;
        private Mock<IFactory<User>> _factoryUser;

        private UsersController _usersController;

        [SetUp]
        public void SetUp()
        {
            _jsonFieldsSerializer = new Mock<IJsonFieldsSerializer>();
            _aclService = new Mock<IAclService>();
            _userService = new Mock<IUserService>();
            _tenantMappingService = new Mock<ITenantMappingService>();
            _tenantService = new Mock<ITenantService>();
            _userActivityService = new Mock<IUserActivityService>();
            _userApiService = new Mock<IUserApiService>();
            _genericAttributeService = new Mock<IGenericAttributeService>();
            _encryptionService = new Mock<IEncryptionService>();
            _storeService = new Mock<IStoreService>();
            _factoryUser = new Mock<IFactory<User>>();

            _usersController = new UsersController(
                _jsonFieldsSerializer.Object,
                _aclService.Object,
                _userService.Object,
                _tenantMappingService.Object,
                _tenantService.Object,
                _userActivityService.Object,
                _userApiService.Object,
                _genericAttributeService.Object,
                _encryptionService.Object,
                _storeService.Object,
                _factoryUser.Object);
        }

        #region GetUsersCount

        [Test]
        public void Should_return_ok_with_count_equals_zero_when_no_user_exists()
        {
            _jsonFieldsSerializer.Setup(x => x.Serialize(null, null, null)).Returns(String.Empty);
            _userApiService.Setup(x => x.GetUsersCount()).Returns(0);

            var result = _usersController.GetUsersCount().GetAwaiter().GetResult();

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(0, ((UsersCountRootObject)((OkObjectResult)result).Value).Count);
        }

        [Test]
        public void Should_return_ok_with_count_equals_one_when_single_user_exists()
        {
            _jsonFieldsSerializer.Setup(x => x.Serialize(null, null, null)).Returns(String.Empty);
            _userApiService.Setup(x => x.GetUsersCount()).Returns(1);

            var result = _usersController.GetUsersCount().GetAwaiter().GetResult();

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(1, ((UsersCountRootObject)((OkObjectResult)result).Value).Count);
        }

        [Test]
        public void Should_return_ok_with_count_equals_to_same_users_when_certain_number_users_exists()
        {
            _jsonFieldsSerializer.Setup(x => x.Serialize(null, null, null)).Returns(String.Empty);
            _userApiService.Setup(x => x.GetUsersCount()).Returns(666);

            var result = _usersController.GetUsersCount().GetAwaiter().GetResult();

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(666, ((UsersCountRootObject)((OkObjectResult)result).Value).Count);
        }

        #endregion

        #region GetUsers

        [Test]
        public void Should_call_service_with_same_parameters_when_valid_parameters_passed()
        {
            var parameters = new UsersParametersModel
            {
                SinceId = Configurations.DefaultSinceId + 1, // some different than default since id
                CreatedAtMin = DateTime.Now,
                CreatedAtMax = DateTime.Now,
                Page = Configurations.DefaultPageValue + 1, // some different than default page
                Limit = Configurations.MinLimit + 1, // some different than default limit
            };

            _usersController.GetUsers(parameters).GetAwaiter().GetResult();

            _userApiService.Verify(x => x.GetUserDtos(parameters.CreatedAtMin, parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId, null, null));
        }

        [Test]
        public void Should_call_serializer_with_these_users_when_some_users_exist()
        {
            var returnedUsersDtoCollection = new List<UserDto>
            {
                new UserDto(),
                new UserDto()
            };

            var parameters = new UsersParametersModel();

            _userApiService
                .Setup(x => x.GetUserDtos(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Limit,
                    parameters.Page, parameters.SinceId, null, null)).Returns(returnedUsersDtoCollection);

            _usersController.GetUsers(parameters).GetAwaiter().GetResult();

            _jsonFieldsSerializer.Verify(x =>
                x.Serialize(It.Is<UsersRootObject>(r => r.Users.Count == returnedUsersDtoCollection.Count),
                    It.IsIn(parameters.Fields), null));
        }

        [Test]
        public void Should_call_serializer_with_no_users_when_no_users_exist()
        {
            var returnedUsersDtoCollection = new List<UserDto>();
            var parameters = new UsersParametersModel();

            _userApiService
                .Setup(x => x.GetUserDtos(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Limit,
                    parameters.Page, parameters.SinceId, null, null)).Returns(returnedUsersDtoCollection);

            _usersController.GetUsers(parameters).GetAwaiter().GetResult();

            _jsonFieldsSerializer.Verify(x =>
                x.Serialize(It.Is<UsersRootObject>(r => r.Users.Count == returnedUsersDtoCollection.Count),
                    It.IsIn(parameters.Fields), null));
        }

        [Test]
        public void Should_call_serializer_with_same_fields_when_fields_passed()
        {
            var parameters = new UsersParametersModel()
            {
                Fields = "id,email"
            };

            _usersController.GetUsers(parameters).GetAwaiter().GetResult();

            _jsonFieldsSerializer.Verify(x => x.Serialize(It.IsAny<UsersRootObject>(), It.IsIn(parameters.Fields), null));
        }

        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void Should_return_bad_request_when_invalid_limit_passed(int invalidLimit)
        {
            var parameters = new UsersParametersModel()
            {
                Limit = invalidLimit
            };

            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<UsersRootObject>(), It.IsAny<string>(), null))
                .Returns(String.Empty);

            var result = _usersController.GetUsers(parameters).GetAwaiter().GetResult();
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void Should_return_bad_request_when_invalid_page_passed(int invalidPage)
        {
            var parameters = new UsersParametersModel()
            {
                Page = invalidPage
            };

            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<UsersRootObject>(), It.IsAny<string>(), null))
                .Returns(String.Empty);

            var result = _usersController.GetUsers(parameters).GetAwaiter().GetResult();
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        #endregion

        #region GetUserById

        [Test]
        public void Should_return_404_when_users_not_existed()
        {
            int nonExistingUserId = 5;

            _userApiService.Setup(x => x.GetUserById(nonExistingUserId, false));
            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<UsersRootObject>(), It.IsAny<string>(), null))
                .Returns(String.Empty);

            var result = _usersController.GetUserById(nonExistingUserId).GetAwaiter().GetResult();
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void Should_return_bad_request_when_id_equals_0_or_less(int nonPositiveUserId)
        {
            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<UsersRootObject>(), It.IsAny<string>(), null))
                .Returns(String.Empty);

            var result = _usersController.GetUserById(nonPositiveUserId).GetAwaiter().GetResult();
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);

        }

        [Test]
        public void Should_serialize_user_when_id_equals_to_existing_user_id()
        {
            int existingUserId = 5;
            var existingUserDto = new UserDto {Id = existingUserId};

            _userApiService.Setup(x => x.GetUserById(existingUserId, false)).Returns(existingUserDto);

            _usersController.GetUserById(existingUserId).GetAwaiter().GetResult();

            _jsonFieldsSerializer.Verify(x => x.Serialize(It.Is<UsersRootObject>(s => s.Users[0] == existingUserDto),
                It.Is<string>(fields => fields == ""), null));
        }

        [Test]
        public void Should_return_json_with_specified_fields_when_id_equals_to_existing_user_id_and_fields()
        {
            int existingUserId = 5;
            var existingUserDto = new UserDto { Id = existingUserId };
            string fields = "id,email";

            _userApiService.Setup(x => x.GetUserById(existingUserId, false)).Returns(existingUserDto);

            _usersController.GetUserById(existingUserId, fields).GetAwaiter().GetResult();

            _jsonFieldsSerializer.Verify(x => x.Serialize(It.Is<UsersRootObject>(s => s.Users[0] == existingUserDto),
                It.Is<string>(fieldsParameter => fieldsParameter == fields), null));
        }

        #endregion

        #region Search

        [Test]
        public void Should_call_service_with_default_parameters_when_no_parameters_passed()
        {
            var defaultParametersModel = new UsersSearchParametersModel();

            _usersController.Search(defaultParametersModel).GetAwaiter().GetResult();

            _userApiService.Verify(x => x.Search(defaultParametersModel.Query, defaultParametersModel.Order,
                defaultParametersModel.Page, defaultParametersModel.Limit));
        }

        [Test]
        public void Should_call_serializer_when_no_parameters_passed_and_users_exist()
        {
            var expectedUsersCollection = new List<UserDto>
            {
                new UserDto(),
                new UserDto(),
            };

            var expectedRootObject = new UsersRootObject
            {
                Users = expectedUsersCollection
            };

            var defaultParameters = new UsersSearchParametersModel();

            _jsonFieldsSerializer.Setup(x => x.Serialize(expectedRootObject, defaultParameters.Fields, null));
            _userApiService.Setup(x => x.Search(null, null, 1, 50)).Returns(expectedUsersCollection);

            _usersController.Search(defaultParameters).GetAwaiter().GetResult();

            _jsonFieldsSerializer.Verify(x =>
                x.Serialize(It.IsAny<UsersRootObject>(), It.IsIn(defaultParameters.Fields), null));
        }

        [Test]
        public void Should_call_serializer_when_no_parameters_passed_and_no_user_exists()
        {
            var expectedUsersCollection = new List<UserDto>();

            var expectedRootObject = new UsersRootObject
            {
                Users = expectedUsersCollection
            };

            var defaultParameters = new UsersSearchParametersModel();

            _jsonFieldsSerializer.Setup(x => x.Serialize(expectedRootObject, defaultParameters.Fields, null));
            _userApiService.Setup(x => x.Search(null, null, 1, 50)).Returns(expectedUsersCollection);

            _usersController.Search(defaultParameters).GetAwaiter().GetResult();

            _jsonFieldsSerializer.Verify(x =>
                x.Serialize(It.IsAny<UsersRootObject>(), It.IsIn(defaultParameters.Fields), null));
        }

        [Test]
        public void Should_call_serializer_with_same_fields_when_fields_parameters_passed()
        {
            var defaultParametersModel = new UsersSearchParametersModel
            {
                Fields = "id, mail"
            };

            _usersController.Search(defaultParametersModel).GetAwaiter().GetResult();

            _jsonFieldsSerializer.Verify(x =>
                x.Serialize(It.IsAny<UsersRootObject>(), It.IsIn(defaultParametersModel.Fields), null));
        }

        [Test]
        [TestCase(Configurations.MinLimit)]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void Should_return_bad_request_when_invalid_limit_is_passed(int invalidLimit)
        {
            var parametersModel = new UsersSearchParametersModel()
            {
                Limit = invalidLimit
            };

            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<UsersRootObject>(), It.IsAny<string>(), null))
                .Returns(String.Empty);

            var result = _usersController.Search(parametersModel).GetAwaiter().GetResult();
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void Should_return_bad_request_when_non_positive_page_passed(int nonPositivePage)
        {
            var parametersModel = new UsersSearchParametersModel()
            {
                Page = nonPositivePage
            };

            _jsonFieldsSerializer.Setup(x => x.Serialize(It.IsAny<UsersRootObject>(), It.IsAny<string>(), null))
                .Returns(String.Empty);

            var result = _usersController.Search(parametersModel).GetAwaiter().GetResult();
            var statusCode = ActionResultExecutor.ExecuteResult(result);

            Assert.AreEqual(HttpStatusCode.BadRequest, statusCode);
        }

        #endregion
    }
}