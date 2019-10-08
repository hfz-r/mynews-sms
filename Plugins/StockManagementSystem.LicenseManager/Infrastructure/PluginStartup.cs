using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.LicenseManager.Data;
using StockManagementSystem.Web.Infrastructure.Extensions;

namespace StockManagementSystem.LicenseManager.Infrastructure
{
    public class PluginStartup : IBaseStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LicenseObjectContext>(builder =>
            {
                builder.UseSqlServerWithLazyLoading(services);
            });
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 11;
    }
}