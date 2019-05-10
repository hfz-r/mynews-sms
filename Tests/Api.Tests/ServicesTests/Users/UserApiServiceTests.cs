using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Api.Infrastructure.Mapper;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Tenants;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Users;
using Tests;

namespace Api.Tests.ServicesTests.Users
{
    [TestFixture]
    public class UserApiServiceTests
    {
        private Mock<IRepository<User>> _userRepository;
        private Mock<IRepository<UserRole>> _userRoleRepository;
        private Mock<IRepository<UserStore>> _userStoreRepository;
        private Mock<IRepository<GenericAttribute>> _genericAttributeRepository;
        private Mock<ITenantContext> _tenantContext;
        private Mock<IUserService> _userService;
        private UserApiService _userApiService;
        private DateTime _baseDate;
        private Tenant _tenant;

        [SetUp]
        public void SetUp()
        {
            _baseDate = new DateTime(2018, 6, 6);

            #region User

            _userRepository = new Mock<IRepository<User>>();

            var user1 = new User
            {
                Id = 1,
                Username = "user1@test.com",
                Email = "user1@test.com",
                Active = false,
                CreatedOnUtc = _baseDate.AddMonths(4),
            };

            var user2 = new User
            {
                Id = 2,
                Username = "21",
                Email = "user2@test.com",
                Active = true,
                CreatedOnUtc = _baseDate.AddMonths(8),
            };

            var mockUsers = new List<User> { user1, user2 }.AsQueryable().BuildMockDbSet();
            _userRepository.Setup(x => x.Table).Returns(mockUsers.Object);

            #endregion

            #region GenericAttribute

            _genericAttributeRepository = new Mock<IRepository<GenericAttribute>>();

            var genericAttibute1 = new GenericAttribute()
            {
                Id = 1,
                KeyGroup = "User",
                Key = "FirstName",
                Value = "John",
                EntityId = 1,
                TenantId = 0,
            };

            var genericAttibute2 = new GenericAttribute()
            {
                Id = 2,
                KeyGroup = "User",
                Key = "LastName",
                Value = "Babuci",
                EntityId = 1,
                TenantId = 0,
            };

            var genericAttibute3 = new GenericAttribute()
            {
                Id = 3,
                KeyGroup = "User",
                Key = "FirstName",
                Value = "Administrator",
                EntityId = 2,
                TenantId = 0,
            };

            //var genericAttibute4 = new GenericAttribute()
            //{
            //    Id = 4,
            //    KeyGroup = "User",
            //    Key = "LastName",
            //    Value = "Eno",
            //    EntityId = 2,
            //    TenantId = 0,
            //};

            var mockGenericAttributes = new List<GenericAttribute> { genericAttibute1, genericAttibute2, genericAttibute3 }
                .AsQueryable().BuildMockDbSet();
            _genericAttributeRepository.Setup(x => x.Table).Returns(mockGenericAttributes.Object);

            #endregion

            #region Tenant

            _tenant = new Tenant()
            {
                Id = 1,
                Name = "MyStore",
                Url = "localhost/mystore",
                Hosts = "mystore.com",
                SslEnabled = false,
            };
            _tenantContext = new Mock<ITenantContext>();
            _tenantContext.Setup(x => x.CurrentTenant).Returns(_tenant);

            #endregion

            _userApiService = new UserApiService(
                _tenantContext.Object, 
                _userRepository.Object, 
                (_userRoleRepository = new Mock<IRepository<UserRole>>()).Object,
                (_userStoreRepository = new Mock<IRepository<UserStore>>()).Object,
                _genericAttributeRepository.Object, 
                (_userService = new Mock<IUserService>()).Object);

            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<User, UserDto>();
        }

        [Test]
        public void Get_users_call_with_default_parameters()
        {
            var usersResult = _userApiService.GetUserDtos();

            usersResult.ShouldNotBeNull();
            usersResult.Count.ShouldEqual(2);
            usersResult[0].Email.ShouldEqual("user1@test.com");
            usersResult[1].Username.ShouldEqual("user2@test.com");
        }

        [Test]
        public void Get_users_with_limit_parameters()
        {
            var usersResult = _userApiService.GetUserDtos(limit: 1);

            usersResult.ShouldNotBeNull();
            usersResult.Count.ShouldEqual(1);
            usersResult[0].Email.ShouldEqual("user1@test.com");
        }

        [Test]
        public void Get_users_with_since_id_parameters()
        {
            var usersResult = _userApiService.GetUserDtos(sinceId: 1);

            usersResult.ShouldNotBeNull();
            usersResult.Count.ShouldEqual(1);
            usersResult[0].Email.ShouldEqual("user2@test.com");
        }

        [Test]
        public void Get_users_with_created_at_parameters()
        {
            var minDate = new DateTime(2019,1,1);
            var usersResult = _userApiService.GetUserDtos(createdAtMin: minDate);

            usersResult.ShouldNotBeNull();
            usersResult.Count.ShouldEqual(1);
            usersResult[0].Username.ShouldEqual("user2@test.com");

            var maxDate = minDate.AddMonths(1);
            usersResult = _userApiService.GetUserDtos(createdAtMax: maxDate);

            usersResult.ShouldNotBeNull();
            usersResult.Count.ShouldEqual(1);
            usersResult[0].Username.ShouldEqual("user1@test.com");
        }

        [Test]
        public void Can_count_users()
        {
            var countResult = _userApiService.GetUsersCount();

            countResult.ShouldEqual(2);
        }

        [Test]
        public void Can_search_users_by_query()
        {
            var query = "first_name:administrator";
            var usersResult = _userApiService.Search(query);

            usersResult.Count.ShouldEqual(1);
            usersResult.First().Username.ShouldEqual("21");

            // can passed by " " but not applicable for the url = search=1 per txt
            query = "first_name:john last_name:babuci";
            usersResult = _userApiService.Search(query);

            usersResult.Count.ShouldEqual(1);
            usersResult.First().Username.ShouldEqual("user1@test.com");
        }

        [Test]
        public void Can_get_user_entity_by_id()
        {
            var userResult = _userApiService.GetUserEntityById(2);

            userResult.ShouldBe<User>();
            userResult.ShouldBeNotBeTheSameAs(typeof(UserDto));
        }

        [Test]
        public void Can_get_user_dto_by_id()
        {
            var userResult = _userApiService.GetUserById(2);

            userResult.ShouldBe<UserDto>();
            userResult.Email.ShouldEqual("user2@test.com");
        }
    }
}