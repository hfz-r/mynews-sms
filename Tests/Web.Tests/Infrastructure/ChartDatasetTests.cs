using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Services.Logging;
using Tests;

namespace Web.Tests.Infrastructure
{
    public class ChartDataSetTests
    {
        private IStaticCacheManager _cacheManager;
        private Mock<IRepository<ActivityLog>> _activityLogRepository;
        private Mock<IRepository<ActivityLogType>> _activityLogTypeRepository;
        private Mock<IWorkContext> _workContext;
        private Mock<IWebHelper> _webHelper;
        private IUserActivityService _userActivityService;

        private ActivityLogType _activityType1, _activityType2, _activityType3, _activityType4;
        private ActivityLog _activity1, _activity2, _activity3, _activity4, _activity5, _activity6, _activity7, _activity8;
        private User _user1, _user2;

        [SetUp]
        public void SetUp()
        {
            ConstructTestData();
            _cacheManager = new NullCache();
            _workContext = new Mock<IWorkContext>();
            _webHelper = new Mock<IWebHelper>();

            _activityLogRepository = new Mock<IRepository<ActivityLog>>();
            _activityLogRepository.Setup(x => x.Table)
                .Returns(new List<ActivityLog> { _activity1, _activity2, _activity3, _activity4, _activity5, _activity6, _activity7, _activity8 }.AsQueryable());

            _activityLogTypeRepository = new Mock<IRepository<ActivityLogType>>();
            _activityLogTypeRepository.Setup(x => x.Table)
                .Returns(new List<ActivityLogType> { _activityType1, _activityType2, _activityType3, _activityType4 }.AsQueryable());

            _userActivityService = new UserActivityService(_activityLogRepository.Object,
                _activityLogTypeRepository.Object, _cacheManager, _webHelper.Object, _workContext.Object);
        }

        [Test]
        public void Result_should_group_by_entity()
        {
            var data = _userActivityService.GetAllActivities()
                .GroupBy(e => e.EntityName)
                .Select(e => new
                {
                    trans = e.Key,
                    total = e.Count().ToString()
                });

            var groups = new[] {"Role", "Permission", "User"};
            var trans = data.ToList().Select(r => r.trans);

            Assert.That(trans.Where(a => groups.Any(x => x == a)), Is.EquivalentTo(groups));
        }

        public void ConstructTestData()
        {
            #region ActivityLogType

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
            _activityType3 = new ActivityLogType
            {
                Id = 3,
                SystemKeyword = "TestKeyword3",
                Enabled = true,
                Name = "Test name3"
            };
            _activityType4 = new ActivityLogType
            {
                Id = 4,
                SystemKeyword = "TestKeyword4",
                Enabled = true,
                Name = "Test name4"
            };

            #endregion

            #region User

            _user1 = new User
            {
                Id = 1,
                Email = "test1@test1.com",
                UserName = "TestUser1"
            };
            _user2 = new User
            {
                Id = 2,
                Email = "test2@test2.com",
                UserName = "TestUser2",
            };

            #endregion

            #region ActivityLog

            _activity1 = new ActivityLog
            {
                Id = 1,
                ActivityLogType = _activityType1,
                UserId = _user1.Id,
                User = _user1,
                EntityName = "Role"
            };
            _activity2 = new ActivityLog
            {
                Id = 2,
                ActivityLogType = _activityType2,
                UserId = _user2.Id,
                User = _user2,
                EntityName = "Permission"
            };
            _activity3 = new ActivityLog
            {
                Id = 3,
                ActivityLogType = _activityType3,
                UserId = _user1.Id,
                User = _user1,
                EntityName = "User"
            };
            _activity4 = new ActivityLog
            {
                Id = 4,
                ActivityLogType = _activityType4,
                UserId = _user2.Id,
                User = _user2,
                EntityName = "Role"
            };
            _activity5 = new ActivityLog
            {
                Id = 5,
                ActivityLogType = _activityType2,
                UserId = _user2.Id,
                User = _user2,
                EntityName = "Permission"
            };
            _activity6 = new ActivityLog
            {
                Id = 6,
                ActivityLogType = _activityType2,
                UserId = _user2.Id,
                User = _user2,
                EntityName = "Permission"
            };

            _activity7 = new ActivityLog
            {
                Id = 7,
                ActivityLogType = _activityType3,
                UserId = _user1.Id,
                User = _user1,
                EntityName = "User"
            };
            _activity8 = new ActivityLog
            {
                Id = 8,
                ActivityLogType = _activityType2,
                UserId = _user2.Id,
                User = _user2,
                EntityName = "Permission"
            };


            #endregion
        }
    }
}