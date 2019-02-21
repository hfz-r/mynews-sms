using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Web.Infrastructure.Extensions;

namespace StockManagementSystem.Web.Infrastructure
{
    public class AuthenticationStartup : IBaseStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultAuthentication();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseDefaultAuthentication();
        }

        //auth should be loaded before MVC
        public int Order => 500;
    }
}