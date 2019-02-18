﻿using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Api.Extensions;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Web.Infrastructure;

namespace StockManagementSystem.Api
{
    public class ApiStartup : IBaseStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiRequiredConfiguration();
            services.AddBindingRedirectsFallback();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddTokenGenerationPipeline();
            services.AddAuthorizationPipeline();
        }

        public void Configure(IApplicationBuilder application)
        {
            var dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;

            application.ApplyIdentityServerMigrations();
            application.ApplyApiSeedData();

            application.UseUrlRewrite();
            application.UseIdentityServerEntityFramework();
            /* enable rewind so we can read the request body multiple times (this should eventually be refactored, 
             * but both JsonModelBinder and all of the DTO validators need to read this stream) */
            application.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });
        }

        public int Order => new AuthenticationStartup().Order + 1;
    }
}