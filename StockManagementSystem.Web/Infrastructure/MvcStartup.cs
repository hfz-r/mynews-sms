using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Web.Infrastructure.Extensions;
using HibernatingRhinos.Profiler.Appender.EntityFramework;

namespace StockManagementSystem.Web.Infrastructure
{
    public class MvcStartup : IBaseStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //hook-up HibernatingRhinos.Profiler
            EntityFrameworkProfiler.Initialize();

            services.AddDefaultMvc();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseDefaultMvc();
        }

        //MVC should be loaded last
        public int Order => 1000;
    }
}