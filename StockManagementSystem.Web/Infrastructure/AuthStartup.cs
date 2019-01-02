using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Web.Infrastructure.Extensions;

namespace StockManagementSystem.Web.Infrastructure
{
    public class AuthStartup : IBaseStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseAuthentication();
        }

        public int Order
        {
            //auth should be loaded before MVC
            get { return 500; }
        }
    }
}