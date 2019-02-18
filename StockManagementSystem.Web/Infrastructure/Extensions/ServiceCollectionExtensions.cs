using System;
using System.Linq;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using StackExchange.Profiling.Storage;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Http;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Data;
using StockManagementSystem.Services.Authentication;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Plugins;
using StockManagementSystem.Services.Tasks;
using StockManagementSystem.Web.Validators;
using StockManagementSystem.Web.Mvc.ModelBinding;

namespace StockManagementSystem.Web.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            //add Default configuration parameters
            services.ConfigureStartupConfig<DefaultConfig>(configuration.GetSection("Default"));
            //add hosting configuration parameters
            services.ConfigureStartupConfig<HostingConfig>(configuration.GetSection("Hosting"));
            //add accessor to HttpContext
            services.AddHttpContextAccessor();

            // initialize singleton app
            var engine = EngineContext.Create();
            engine.Initialize(services);
            var serviceProvider = engine.ConfigureServices(services, configuration);

            if (!DataSettingsManager.DatabaseIsInstalled)
                return serviceProvider;

            //implement schedule tasks
            //database is already installed, so start scheduled tasks
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();

            //log application start
            engine.Resolve<ILogger>().Information("Application started");

            //install plugins
            engine.Resolve<IPluginService>().InstallPlugins();

            return serviceProvider;
        }

        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services,  IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var config = new TConfig();

            //bind it to the appropriate section of configuration
            configuration.Bind(config);

            //and register it as a service
            services.AddSingleton(config);

            return config;
        }

        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void AddAntiForgery(this IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.AntiforgeryCookie}";
                options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled &&
                                              EngineContext.Current.Resolve<SecuritySettings>().ForceSslForAllPages
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.None;
            });
        }

        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.SessionCookie}";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled &&
                                              EngineContext.Current.Resolve<SecuritySettings>().ForceSslForAllPages
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.None;
            });
        }

        public static void AddObjectContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServerWithLazyLoading(services);
            });
        }

        public static IMvcBuilder AddDefaultMvc(this IServiceCollection services)
        {
            var mvcBuilder = services.AddMvc();

            mvcBuilder.AddMvcOptions(options => options.EnableEndpointRouting = false);
            mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var defaultConfig = services.BuildServiceProvider().GetRequiredService<DefaultConfig>();
            if (defaultConfig.UseSessionStateTempDataProvider)
            {
                //use session-based temp data provider
                mvcBuilder.AddSessionStateTempDataProvider();
            }
            else
            {
                mvcBuilder.AddCookieTempDataProvider(options =>
                {
                    options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.TempDataCookie}";
                    options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled &&
                                                  EngineContext.Current.Resolve<SecuritySettings>().ForceSslForAllPages
                        ? CookieSecurePolicy.SameAsRequest
                        : CookieSecurePolicy.None;
                });
            }

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //add custom model binder provider 
            mvcBuilder.AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new DefaultModelBinderProvider()));

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
            //set default authentication schemes
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = AuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = AuthenticationDefaults.ExternalAuthenticationScheme;
            });

            //add main cookie authentication
            authenticationBuilder.AddCookie(AuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.AuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.LoginPath = AuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = AuthenticationDefaults.AccessDeniedPath;
                options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled &&
                                              EngineContext.Current.Resolve<SecuritySettings>().ForceSslForAllPages
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.None;
            });

            //add external authentication
            authenticationBuilder.AddCookie(AuthenticationDefaults.ExternalAuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{CookieDefaults.Prefix}{CookieDefaults.ExternalAuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.LoginPath = AuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = AuthenticationDefaults.AccessDeniedPath;
                options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled &&
                                              EngineContext.Current.Resolve<SecuritySettings>().ForceSslForAllPages
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.None;
            });

            //register and configure external authentication
            var typeFinder = new WebAppTypeFinder();
            var externalAuthConfigurations = typeFinder.FindClassesOfType<IExternalAuthenticationRegistrar>();
            var instances = externalAuthConfigurations
                .Select(instance => (IExternalAuthenticationRegistrar) Activator.CreateInstance(instance));

            foreach (var instance in instances)
            {
                instance.Configure(authenticationBuilder);
            }
        }
    }
}