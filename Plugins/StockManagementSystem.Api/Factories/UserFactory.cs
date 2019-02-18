using System;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.Factories
{
    public class UserFactory : IFactory<User>
    {
        public User Initialize()
        {
            var defaultUser = new User()
            {
                UserGuid = Guid.NewGuid(),
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                Active = true
            };

            return defaultUser;
        }
    }
}