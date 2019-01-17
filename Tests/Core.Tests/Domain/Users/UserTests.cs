using NUnit.Framework;
using StockManagementSystem.Core.Domain.Identity;
using Tests;

namespace Core.Tests.Domain.Users
{
    [TestFixture]
    public class UserTests
    {
        private Role roleAdmin = new Role
        {
            Name = "Administrators",
            SystemName = IdentityDefaults.AdministratorsRoleName
        };

        private Role roleManager = new Role
        {
            Name = "Manager",
            SystemName = IdentityDefaults.ManagerRoleName
        };

        private Role roleRegistered = new Role
        {
            Name = "Registered",
            SystemName = IdentityDefaults.RegisteredRoleName
        };

        [Test]
        public void Can_check_IsInRole()
        {
            var user = new User();

            var userRole1 = new Role
            {
                Name = "Test name 1",
                SystemName = "Test system name 1"
            };

            var userRole2 = new Role
            {
                Name = "Test name 2",
                SystemName = "Test system name 2"
            };

            user.UserRoles = new[]
            {
                new UserRole {Role = userRole1},
                new UserRole {Role = userRole2},
            };

            user.IsInRole("Test system name 1").ShouldBeTrue();
            user.IsInRole("Test system name 3").ShouldBeFalse();
        }

        [Test]
        public void Can_check_whether_user_is_admin()
        {
            var user = new User();

            user.UserRoles = new[]
            {
                new UserRole {Role = roleRegistered},
                new UserRole {Role = roleManager},
            };

            user.IsAdministrators().ShouldBeFalse();

            user.UserRoles = new[]
            {
                new UserRole {Role = roleAdmin},
            };

            user.IsAdministrators().ShouldBeTrue();
        }

        [Test]
        public void Can_check_whether_user_is_registered()
        {
            var user = new User();

            user.UserRoles = new[]
            {
                new UserRole {Role = roleAdmin},
                new UserRole {Role = roleManager},
            };

            user.IsRegistered().ShouldBeFalse();

            user.UserRoles = new[]
            {
                new UserRole {Role = roleRegistered},
            };

            user.IsRegistered().ShouldBeTrue();
        }
    }
}