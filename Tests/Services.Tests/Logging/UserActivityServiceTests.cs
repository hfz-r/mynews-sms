using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Logging;
using Tests;

namespace Services.Tests.Logging
{
    public class UserActivityServiceTests : ServiceTest
    {
        private IStaticCacheManager _cacheManager;
        private Mock<IRepository<ActivityLog>> _activityLogRepository;
        private Mock<IRepository<ActivityLogType>> _activityLogTypeRepository;
        private Mock<IWorkContext> _workContext;
        private IUserActivityService _userActivityService;
        private ActivityLogType _activityType1, _activityType2;
        private ActivityLog _activity1, _activity2;
        private User _user1, _user2;
        private Mock<IWebHelper> _webHelper;

        [SetUp]
        public new void SetUp()
        {
            _activityType1 = new ActivityLogType
            {
                Id = 1,
                SystemKeyword = "TestKeyword1",
                Enabled = true,
                Name = "Test name1"
            };
            _activityType2 = new ActivityLogType
            {
                Id = 2,
                SystemKeyword = "TestKeyword2",
                Enabled = true,
                Name = "Test name2"
            };
            _user1 = new User
            {
                Id = 1,
                Email = "test1@test1.com",
                Username = "TestUser1",
                Deleted = false,
            };
            _user2 = new User
            {
                Id = 2,
                Email = "test2@test2.com",
                Username = "TestUser2",
                Deleted = false,
            };
            _activity1 = new ActivityLog
            {
                Id = 1,
                ActivityLogType = _activityType1,
                UserId = _user1.Id,
                User = _user1
            };
            _activity2 = new ActivityLog
            {
                Id = 2,
                ActivityLogType = _activityType2,
                UserId = _user2.Id,
                User = _user2
            };
            _cacheManager = new NullCache();
            _workContext = new Mock<IWorkContext>();
            _webHelper = new Mock<IWebHelper>();
            _activityLogRepository = new Mock<IRepository<ActivityLog>>();
            _activityLogTypeRepository = new Mock<IRepository<ActivityLogType>>();
            _activityLogTypeRepository.Setup(x => x.Table)
                .Returns(new List<ActivityLogType> {_activityType1, _activityType2}.AsQueryable());
            _activityLogRepository.Setup(x => x.Table)
                .Returns(new List<ActivityLog> {_activity1, _activity2}.AsQueryable());
            _userActivityService = new UserActivityService(_activityLogRepository.Object,
                _activityLogTypeRepository.Object, null, _cacheManager, _webHelper.Object, _workContext.Object);
        }

        [Test]
        public void Can_Find_Activities()
        {
            var activities = _userActivityService.GetAllActivities(userId: 1, pageSize: 10);
            activities.Contains(_activity1).ShouldBeTrue();

            activities = _userActivityService.GetAllActivities(userId: 2, pageSize: 10);
            activities.Contains(_activity1).ShouldBeFalse();

            activities = _userActivityService.GetAllActivities(userId: 2, pageSize: 10);
            activities.Contains(_activity2).ShouldBeTrue();
        }
    }
}