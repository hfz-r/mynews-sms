using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Data;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Devices;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Locations;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.OrderLimits;
using StockManagementSystem.Services.Roles;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.UI;

namespace StockManagementSystem.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            // file provider
            builder.RegisterType<FileProviderHelper>().As<IFileProviderHelper>().InstancePerLifetimeScope();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();

            // data layer
            builder.Register(context => new ObjectContext(context.Resolve<IHttpContextAccessor>(),
                    context.Resolve<DbContextOptions<ObjectContext>>())).As<IDbContext>().InstancePerLifetimeScope();

            // repositories
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            // cache manager
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();

            // static cache manager
            builder.RegisterType<MemoryCacheManager>().As<ILocker>().As<IStaticCacheManager>().SingleInstance();

            // work context
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerLifetimeScope();

            // services
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerLifetimeScope();
            builder.RegisterType<StartupTime>().As<IStartupTime>()
                .InstancePerLifetimeScope(); // startup time? for what?
            builder.RegisterType<PageHeadBuilder>().As<IPageHeadBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<RoleService>().As<IRoleService>().InstancePerLifetimeScope();
            builder.RegisterType<DeviceService>().As<IDeviceService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderLimitService>().As<IOrderLimitService>().InstancePerLifetimeScope();
            builder.RegisterType<LocationService>().As<ILocationService>().InstancePerLifetimeScope();
            builder.RegisterType<StoreService>().As<IStoreService>().InstancePerLifetimeScope();
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerLifetimeScope();
            builder.RegisterType<AclService>().As<IAclService>().InstancePerLifetimeScope();

            // mvc context accessor
            builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}