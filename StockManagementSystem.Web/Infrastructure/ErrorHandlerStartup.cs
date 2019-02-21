using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Web.Infrastructure.Extensions;

namespace StockManagementSystem.Web.Infrastructure
{
    public class ErrorHandlerStartup : IBaseStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        public void Configure(IApplicationBuilder application)
        {
            //exception handling
            application.UseDefaultExceptionHandler();

            //handle 400 errors (bad request)
            application.UseBadRequestResult();

            //handle 404 errors (not found)
            application.UsePageNotFound();
        }

        //error handlers should be loaded first
        public int Order => 0;
    }
}