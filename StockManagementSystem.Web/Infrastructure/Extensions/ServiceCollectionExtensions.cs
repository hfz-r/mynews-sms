using System;
using StockManagementSystem.Core.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using StackExchange.Profiling.Storage;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Data;
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
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = ".sms.session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        public static void AddObjectContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseLazyLoadingProxies();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"), builder =>
                {
                    builder.MigrationsAssembly("StockManagementSystem");
                    builder.UseRowNumberForPaging();
                });
            });
        }

        public static IMvcBuilder AddDefaultMvc(this IServiceCollection services)
        {
            var mvcBuilder = services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            mvcBuilder.AddCookieTempDataProvider(options =>
            {
                options.Cookie.Name = "sms.tempdata";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //add custom model binder provider 
            mvcBuilder.AddMvcOptions(
                options => options.ModelBinderProviders.Insert(0, new DefaultModelBinderProvider()));

            return mvcBuilder;
        }

        public static void AddDefaultMiniProfiler(this IServiceCollection services)
        {
            services.AddMiniProfiler(miniProfilerOptions =>
            {
                //use memory cache provider for storing each result
                (miniProfilerOptions.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(60);
            }).AddEntityFramework();
        }

        public static void AddIdentity(this IServiceCollection services)
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
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
        }
    }
}