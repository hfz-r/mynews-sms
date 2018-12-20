using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Data
{
    public static class SeedData
    {
        public static async void Initialize(IServiceProvider service)
        {
            var userManager = service.GetRequiredService<UserManager<User>>();
            var roleManager = service.GetRequiredService<RoleManager<Role>>();

            if (roleManager.Roles.Any())
                return;
            //if (userManager.Users.Any())
            //    return;

            await roleManager.SeedRole();
        }

        public static async Task SeedRole(this RoleManager<Role> roleManager)
        {
            var roles = new Role[]
            {
                new Role {Name = "Admin", Description = "I am admin. I can perform all the operations."},
                new Role {Name = "User", Description = "I am user. I can perform normal operations."},
                new Role {Name = "Customer Service", Description = "I am customer service. I can perform services operations."},
                new Role {Name = "Auditor", Description = "I am auditor. I can perform audit operations."},
                new Role {Name = "DBA", Description = "I am DBA. I can perform DBA operations."},
                new Role {Name = "Manager", Description = "I am manager. I can perform manager operations."},
            };
            try
            {
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }
            catch
            {
                //ignore
            }
        }
    }
}