using System;
using System.Globalization;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Api.Authorization.Policies;
using StockManagementSystem.Api.Authorization.Requirements;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.IdentityServer.Infrastructure;
using StockManagementSystem.Api.IdentityServer.Middlewares;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Api.Extensions
{
    /// <summary>
    /// Represent extensions for the api service providers
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void AddApiRequiredConfiguration(this IServiceCollection services)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        public static void AddBindingRedirectsFallback(this IServiceCollection services)
        {
            RedirectAssembly("Microsoft.AspNetCore.DataProtection.Abstractions", new Version(2, 0, 0, 0), "adb9793829ddae60");
        }

        public static void AddDefaultServices(this IServiceCollection services)
        {
            services.AddTransient<ActivateApiUserMiddleware>();
        }

        /// <summary>
        /// Token generator
        /// </summary>
        /// <param name="services"></param>
        public static void AddTokenGenerationPipeline(this IServiceCollection services)
        {
            var signingKey = CryptoHelper.CreateRsaSecurityKey();
            var dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;

            var migrationsAssembly = typeof(ApiStartup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer(options => options.UserInteraction.LoginUrl = "/login")
                .AddSigningCredential(signingKey)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(dataSettings.DataConnectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(dataSettings.DataConnectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAuthorizeInteractionResponseGenerator<ApiAuthorizeInteractionResponseGenerator>();
        }

        public static void AddAuthorizationPipeline(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiDefaultPolicy",
                    policy =>
                    {
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        policy.Requirements.Add(new ActiveApiRequirement());
                        policy.Requirements.Add(new AuthorizationSchemeRequirement());
                        policy.Requirements.Add(new ActiveClientRequirement());
                        //policy.Requirements.Add(new ActiveLicenseRequirement());
                        policy.RequireAuthenticatedUser();
                    });
            });

            services.AddSingleton<IAuthorizationHandler, ActiveApiAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, ValidSchemeAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, ActiveClientAuthorizationPolicy>();
            //services.AddSingleton<IAuthorizationHandler, LicenseAuthorizationPolicy>();
        }

        #region Private methods

        private static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
        {
            Assembly Handler(object sender, ResolveEventArgs args)
            {
                // Use latest strong name & version when trying to load SDK assemblies
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != shortName)
                    return null;

                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                AppDomain.CurrentDomain.AssemblyResolve -= Handler;

                return Assembly.Load(requestedAssembly);
            }

            AppDomain.CurrentDomain.AssemblyResolve += Handler;
        }

        #endregion
    }
}