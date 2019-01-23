using System;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using StackExchange.Profiling.Storage;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Data;
using StockManagementSystem.Services.Authentication;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Web.Validators;
using StockManagementSystem.Web.Mvc.ModelBinding;

namespace StockManagementSystem.Web.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider ConfigureApplicationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            // TODO: custom config
            services.AddHttpContextAccessor();

            // initialize singleton app
            var engine = EngineContext.Create();
            engine.Initialize(services);
            var serviceProvider = engine.ConfigureServices(services, configuration);
         
            return serviceProvider;
        }

        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void AddAntiForgery(this IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = ".sms.antiforgery";
                options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
            });
        }

        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = ".sms.session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
            });
        }

        public static void AddObjectContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ObjectContext>(optionsBuilder =>
            {
                var dataSettings = DataSettingsManager.LoadSettings();
                if (!dataSettings?.IsValid ?? true)
                    return;

                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseSqlServer(dataSettings.DataConnectionString,
                    builder => builder.MigrationsAssembly("StockManagementSystem"));
            });
        }

        public static IMvcBuilder AddDefaultMvc(this IServiceCollection services)
        {
            var mvcBuilder = services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            mvcBuilder.AddCookieTempDataProvider(options =>
            {
                options.Cookie.Name = ".sms.tempdata";
                options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
            });

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //add custom model binder provider 
            mvcBuilder.AddMvcOptions(
                options => options.ModelBinderProviders.Insert(0, new DefaultModelBinderProvider()));

            //add fluent validation
            mvcBuilder.AddFluentValidation(configuration =>
            {
                configuration.ValidatorFactoryType = typeof(ValidatorFactory);
                configuration.ImplicitlyValidateChildProperties = true;
            });

            return mvcBuilder;
        }

        public static void AddDefaultMiniProfiler(this IServiceCollection services)
        {
            services.AddMiniProfiler(miniProfilerOptions =>
            {
                //use memory cache provider for storing each result
                ((MemoryCacheStorage) miniProfilerOptions.Storage).CacheDuration = TimeSpan.FromMinutes(60);

                //TODO: setup miniprofiler
            }).AddEntityFramework();
        }

        public static void AddDefaultAuthentication(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>()
                .AddCustomStores()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            //set default authentication schemes
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = AuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = AuthenticationDefaults.ExternalAuthenticationScheme;
            });

            //add main cookie authentication
            authenticationBuilder.AddCookie(AuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = ".sms.authentication";
                options.Cookie.HttpOnly = true;
                options.LoginPath = AuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = AuthenticationDefaults.AccessDeniedPath;
                options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
            });

            //add external authentication
            authenticationBuilder.AddCookie(AuthenticationDefaults.ExternalAuthenticationScheme, options =>
            {
                options.Cookie.Name = ".sms.externalauthentication";
                options.Cookie.HttpOnly = true;
                options.LoginPath = AuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = AuthenticationDefaults.AccessDeniedPath;
                options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
            });

            //TODO: register and configure external authentication; facebook, twitter, etc.
        }
    }
}