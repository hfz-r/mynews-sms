using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Web.Infrastructure.Extensions;

namespace StockManagementSystem.Web.Infrastructure
{
    public class MvcStartup : IBaseStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //add MiniProfiler services
            services.AddDefaultMiniProfiler();

            services.AddDefaultMvc();
        }

        public void Configure(IApplicationBuilder application)
        {
            //add MiniProfiler
            application.UseMiniProfiler();
            
            application.UseDefaultMvc();
        }

        public int Order
        {
            //MVC should be loaded last
            get { return 1000; }
        }
    }
}