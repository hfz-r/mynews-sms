using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Infrastructure.DependencyManagement;
using StockManagementSystem.Data;
using StockManagementSystem.Services.Authentication;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Events;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Installation;
using StockManagementSystem.Services.Locations;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Management;
using StockManagementSystem.Services.Media;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.OrderLimits;
using StockManagementSystem.Services.Plugins;
using StockManagementSystem.Services.PushNotifications;
using StockManagementSystem.Services.Replenishments;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Settings;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Mvc.Routing;
using StockManagementSystem.Web.UI;

namespace StockManagementSystem.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, DefaultConfig config)
        {
            // file provider
            builder.RegisterType<FileProviderHelper>().As<IFileProviderHelper>().InstancePerLifetimeScope();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();

            //data layer
            builder.RegisterType<DataProviderManager>().As<IDataProviderManager>().InstancePerDependency();
            builder.Register(context => context.Resolve<IDataProviderManager>().DataProvider).As<IDataProvider>().InstancePerDependency();
            builder.Register(context => new ObjectContext(context.Resolve<DbContextOptions<ObjectContext>>())).As<IDbContext>().InstancePerLifetimeScope();

            // repositories
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            //plugins
            builder.RegisterType<PluginService>().As<IPluginService>().InstancePerLifetimeScope();

            // cache manager
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();

            // static cache manager
            builder.RegisterType<MemoryCacheManager>().As<ILocker>().As<IStaticCacheManager>().SingleInstance();

            // work context
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerLifetimeScope();

            //tenant context
            builder.RegisterType<TenantContext>().As<ITenantContext>().InstancePerLifetimeScope();

            // services
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerLifetimeScope();
            builder.RegisterType<StartupTime>().As<IStartupTime>().InstancePerLifetimeScope(); // startup time? for what?
            builder.RegisterType<PageHeadBuilder>().As<IPageHeadBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<UserRegistrationService>().As<IUserRegistrationService>().InstancePerLifetimeScope();
            builder.RegisterType<DeviceService>().As<IDeviceService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderLimitService>().As<IOrderLimitService>().InstancePerLifetimeScope();
            builder.RegisterType<ReplenishmentService>().As<IReplenishmentService>().InstancePerLifetimeScope();
            builder.RegisterType<PushNotificationService>().As<IPushNotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<OutletManagementService>().As<IOutletManagementService>().InstancePerLifetimeScope();
            builder.RegisterType<LocationService>().As<ILocationService>().InstancePerLifetimeScope();
            builder.RegisterType<FormatSettingService>().As<IFormatSettingService>().InstancePerLifetimeScope();
            builder.RegisterType<StoreService>().As<IStoreService>().InstancePerLifetimeScope();
            builder.RegisterType<StoreMappingService>().As<IStoreMappingService>().InstancePerLifetimeScope();
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerLifetimeScope();
            builder.RegisterType<AclService>().As<IAclService>().InstancePerLifetimeScope();
            builder.RegisterType<TenantService>().As<ITenantService>().InstancePerLifetimeScope();
            builder.RegisterType<TenantMappingService>().As<ITenantMappingService>().InstancePerLifetimeScope();
            builder.RegisterType<UserActivityService>().As<IUserActivityService>().InstancePerLifetimeScope();
            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerLifetimeScope();
            builder.RegisterType<CookieAuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerLifetimeScope();
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<PictureService>().As<IPictureService>().InstancePerLifetimeScope();
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();

            //register all settings
            builder.RegisterSource(new SettingsSource());

            //installation service
            if (!DataSettingsManager.DatabaseIsInstalled)
            {
                //TODO: if (config.UseFastInstallationService) = use sql-script

                if (!config.UseFastInstallationService)
                    builder.RegisterType<CodeFirstInstallationService>().As<IInstallationService>().InstancePerLifetimeScope();
            }

            //event consumers
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type) criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IConsumer<>)))
                    .InstancePerLifetimeScope();
            }
        }

        public int Order => 0;
    }

    public class SettingsSource : IRegistrationSource
    {
        private static readonly MethodInfo BuildMethod =
            typeof(SettingsSource).GetMethod("BuildRegistration", BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration) buildMethod.Invoke(null, null);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((context, parameters) =>
                {
                    var currentTenantId = context.Resolve<ITenantContext>().CurrentTenant.Id;
                    return context.Resolve<ISettingService>().LoadSetting<TSettings>(currentTenantId);
                })
                .InstancePerLifetimeScope()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents => false;
    }
}