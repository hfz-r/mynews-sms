using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;

namespace StockManagementSystem.Data
{
    public static class Seed
    {
        public static async Task Init(IServiceProvider service)
        {
            var roleManager = service.GetRequiredService<RoleManager<Role>>();
            await InitRolesSeed(roleManager, out var roles);

            var userManager = service.GetRequiredService<UserManager<User>>();
            if (!userManager.Users.Any())
                await InitUsersSeed(userManager, roles);

            var permission = service.GetRequiredService<IPermissionService>();
            if (!permission.GetAllPermissions().GetAwaiter().GetResult().Any())
                await InitDefaultPermission(permission);

            var userActivity = service.GetRequiredService<IUserActivityService>();
            if (!userActivity.GetAllActivityTypesAsync().GetAwaiter().GetResult().Any())
                await InitDefaultActivityTypes(userActivity);
        }

        private static Task InitRolesSeed(RoleManager<Role> roleManager, out List<Role> roles)
        {
            var urAdministrator = new Role()
            {
                Name = "Administrators",
                Description = "Can perform all of the operations.",
                SystemName = IdentityDefaults.AdministratorsRoleName,
                CreatedBy = "system",
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedBy = "system",
                ModifiedOnUtc = DateTime.UtcNow,
            };
            var urManager = new Role()
            {
                Name = "Manager",
                Description = "Can perform management operations.",
                SystemName = IdentityDefaults.ManagerRoleName,
                CreatedBy = "system",
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedBy = "system",
                ModifiedOnUtc = DateTime.UtcNow,
            };
            var urRegistered = new Role()
            {
                Name = "Registered",
                Description = "Can perform normal operations.",
                SystemName = IdentityDefaults.RegisteredRoleName,
                CreatedBy = "system",
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedBy = "system",
                ModifiedOnUtc = DateTime.UtcNow,
            };

            roles = new List<Role> {urAdministrator, urManager, urRegistered};
            if (!roleManager.Roles.Any())
            {
                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).GetAwaiter().GetResult();
                }
            }

            return Task.CompletedTask;
        }

        private static async Task InitUsersSeed(UserManager<User> userManager, List<Role> roles)
        {
            //admin user
            var adminUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = "admin@default.cum",
                Name = "Admin",
                UserName = "admin",
                CreatedBy = "system",
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedBy = "system",
                ModifiedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                LastLoginDateUtc = DateTime.UtcNow,
            };

            var au = await userManager.CreateAsync(adminUser, "admin123");
            if (au.Succeeded)
            {
                await userManager.AddToRolesAsync(adminUser, roles.Select(role => role.Name).ToArray());
            }

            //second user
            var secondUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = "user2@test.cum",
                Name = "Second User",
                UserName = "user2",
                CreatedBy = "system",
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedBy = "system",
                ModifiedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            var su = await userManager.CreateAsync(secondUser, "user2123");
            if (su.Succeeded)
            {
                await userManager.AddToRolesAsync(secondUser,
                    roles.Where(role => role.Name.Equals("Registered")).Select(role => role.Name).ToArray());
            }
        }

        private static async Task InitDefaultPermission(IPermissionService permission)
        {
            var permissionProviders = new List<Type> {typeof(StandardPermissionProvider)};
            foreach (var providerType in permissionProviders)
            {
                var provider = (IPermissionProvider) Activator.CreateInstance(providerType);
                await permission.InstallPermissionsAsync(provider);
            }
        }

        private static async Task InitDefaultActivityTypes(IUserActivityService userActivity)
        {
            var activityLogTypes = new List<ActivityLogType>
            {
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUser",
                    Enabled = true,
                    Name = "Add a new user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewRole",
                    Enabled = true,
                    Name = "Add a new role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUser",
                    Enabled = true,
                    Name = "Delete a user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteRole",
                    Enabled = true,
                    Name = "Delete a role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUser",
                    Enabled = true,
                    Name = "Edit a user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditRole",
                    Enabled = true,
                    Name = "Edit a role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPermission",
                    Enabled = false,
                    Name = "Edit a permissions"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Login",
                    Enabled = false,
                    Name = "Login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Login1stTime",
                    Enabled = true,
                    Name = "First time login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Logout",
                    Enabled = true,
                    Name = "Logout"
                },
            };
            await userActivity.InsertActivityTypesAsync(activityLogTypes);
        }
    }
}