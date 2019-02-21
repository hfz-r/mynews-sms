using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Web.Infrastructure.Extensions;

namespace StockManagementSystem.Web.Infrastructure
{
    public class CommonStartup : IBaseStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //compression
            services.AddResponseCompression();

            //add options feature
            services.AddOptions();

            //add memory cache
            services.AddMemoryCache();

            //add distributed memory cache
            services.AddDistributedMemoryCache();

            //add HTTP sesion state feature
            services.AddHttpSession();

            //add anti-forgery
            services.AddAntiForgery();

            //add localization
            //services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") };
            });
        }

        public void Configure(IApplicationBuilder application)
        {
            //use response compression
            application.UseResponseCompression();

            //use static files feature
            application.UseDefaultStaticFiles();

            //check whether requested page is keep alive page
            application.UseKeepAlive();

            //use database installation checker
            application.UseInstallUrl();

            //use HTTP session
            application.UseSession();

            //use request localization
            application.UseRequestLocalization();

            //use startup time?
            application.UseStartupTime();
        }

        //common services should be loaded after error handlers
        public int Order => 100;
    }
}