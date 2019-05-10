using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Factories
{
    public class RoleFactory : IFactory<Role>
    {
        public Role Initialize()
        {
            var defaultRole = new Role
            {
                Active = true,
                EnablePasswordLifetime = false,
                IsSystemRole = false,
            };

            return defaultRole;
        }
    }
}