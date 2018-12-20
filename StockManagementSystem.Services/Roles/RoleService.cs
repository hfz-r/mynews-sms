using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IRepository<Role> _roleRepository;

        public RoleService(
            RoleManager<Role> roleManager, 
            IRepository<Role> roleRepository)
        {
            _roleManager = roleManager;
            _roleRepository = roleRepository;
        }

        public async Task<IList<Role>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            if (roles == null)
                return Array.Empty<Role>();

            return roles;
        }
    }
}