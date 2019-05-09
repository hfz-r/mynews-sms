using NUnit.Framework;
using StockManagementSystem.Core.Domain.Users;
using Tests;

namespace Core.Tests.Domain.Users
{
    [TestFixture]
    public class UserTests
    {
        private Role roleAdmin = new Role
        {
            Active = true,
            Name = "Administrators",
            SystemName = UserDefaults.AdministratorsRoleName
        };

        private Role roleGuest = new Role
        {
            Active = true,
            Name = "Cashier",
            SystemName = UserDefaults.GuestsRoleName
        };

        private Role roleRegistered = new Role
        {
            Active = true,
            Name = "Registered",
            SystemName = UserDefaults.RegisteredRoleName
        };

        [Test]
        public void Can_check_IsInRole()
        {
            var user = new User();

            var userRole1 = new Role
            {
                Active = true,
                Name = "Test name 1",
                SystemName = "Test system name 1"
            };

            var userRole2 = new Role
            {
                Active = false,
                Name = "Test name 2",
                SystemName = "Test system name 2"
            };

            user.AddUserRole(new UserRole {Role = userRole1});
            user.AddUserRole(new UserRole {Role = userRole2});

            user.IsInRole("Test system name 1", false).ShouldBeTrue();
            user.IsInRole("Test system name 1").ShouldBeTrue();

            user.IsInRole("Test system name 2", false).ShouldBeTrue();
            user.IsInRole("Test system name 2").ShouldBeFalse();

            user.IsInRole("Test system name 3", false).ShouldBeFalse();
            user.IsInRole("Test system name 3").ShouldBeFalse();
        }

        [Test]
        public void Can_check_whether_user_is_admin()
        {
            var user = new User();

            user.AddUserRole(new UserRole {Role = roleRegistered});

            user.AddUserRole(new UserRole { Role = roleGuest });

            user.IsAdmin().ShouldBeFalse();

            user.AddUserRole(new UserRole { Role = roleAdmin });

            user.IsAdmin().ShouldBeTrue();
        }

        [Test]
        public void Can_check_whether_user_is_guest()
        {
            var user = new User();

            user.AddUserRole(new UserRole { Role = roleRegistered });

            user.AddUserRole(new UserRole { Role = roleAdmin });

            user.IsGuest().ShouldBeFalse();

            user.AddUserRole(new UserRole { Role = roleGuest });

            user.IsGuest().ShouldBeTrue();
        }

        [Test]
        public void Can_check_whether_user_is_registered()
        {
            var user = new User();

            user.AddUserRole(new UserRole { Role = roleAdmin });

            user.AddUserRole(new UserRole { Role = roleGuest });

            user.IsRegistered().ShouldBeFalse();

            user.AddUserRole(new UserRole { Role = roleRegistered });

            user.IsRegistered().ShouldBeTrue();
        }
    }
}