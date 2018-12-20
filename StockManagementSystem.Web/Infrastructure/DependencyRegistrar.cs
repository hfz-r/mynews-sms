using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Data;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.Roles;
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

            // services
            builder.RegisterType<AuthMessageSender>().As<IEmailSender>().InstancePerLifetimeScope();  // email sender?
            builder.RegisterType<StartupTime>().As<IStartupTime>().InstancePerLifetimeScope(); // startup time?
            builder.RegisterType<PageHeadBuilder>().As<IPageHeadBuilder>().InstancePerLifetimeScope();
            builder.RegisterType<RoleService>().As<IRoleService>().InstancePerLifetimeScope();

            // mvc context accessor
            builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}