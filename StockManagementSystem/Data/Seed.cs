﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Services.Security;

namespace StockManagementSystem.Data
{
    public static class Seed
    {
        public static async Task Init(IServiceProvider service)
        {
            var roleManager = service.GetRequiredService<RoleManager<Role>>();
            var userManager = service.GetRequiredService<UserManager<User>>();

            if (!roleManager.Roles.Any() && !userManager.Users.Any())
                await InitIdentitySeed(roleManager, userManager);

            var permission = service.GetRequiredService<IPermissionService>();
            if (!permission.GetAllPermissions().Result.Any())
                await InitDefaultPermission(permission);
        }

        private static async Task InitIdentitySeed(RoleManager<Role> roleManager, UserManager<User> userManager)
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

            var roles = new List<Role> {urAdministrator, urManager, urRegistered};
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

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

            };

            var au = await userManager.CreateAsync(adminUser, "admin123");
            if (au.Succeeded)
            {
                await userManager.AddToRolesAsync(adminUser, 
                    new[] {urAdministrator.Name, urManager.Name, urRegistered.Name});
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
                await userManager.AddToRolesAsync(secondUser, new[] { urRegistered.Name });
            }
        }

        private static async Task InitDefaultPermission(IPermissionService permission)
        {
            var permissionProviders = new List<Type> { typeof(StandardPermissionProvider) };
            foreach (var providerType in permissionProviders)
            {
                var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
                await permission.InstallPermissionsAsync(provider);
            }
        }
    }
}