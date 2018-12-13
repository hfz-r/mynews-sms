using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Identity;

namespace StockManagementSystem.Extensions
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddCustomStores(this IdentityBuilder builder)
        {
            builder.Services.AddTransient<IUserStore<User>, UserStore>();
            builder.Services.AddTransient<IRoleStore<Role>, RoleStore>();
            return builder;
        }
    }
}
