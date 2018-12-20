﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        }

        public int Order
        {
            //error handlers should be loaded first
            get { return 0; }
        }
    }
}