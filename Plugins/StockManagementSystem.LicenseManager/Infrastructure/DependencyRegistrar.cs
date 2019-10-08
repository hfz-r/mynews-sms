using Autofac;
using Autofac.Core;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Infrastructure.DependencyManagement;
using StockManagementSystem.Data;
using StockManagementSystem.LicenseManager.Data;
using StockManagementSystem.LicenseManager.Domain;
using StockManagementSystem.LicenseManager.Factories;
using StockManagementSystem.LicenseManager.Services;
using StockManagementSystem.Services.License;
using StockManagementSystem.Web.Infrastructure.Extensions;
using StockManagementSystem.Web.Menu;

namespace StockManagementSystem.LicenseManager.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, DefaultConfig config)
        {
            builder.RegisterType<LicenseManager>()
                .As<IAdminMenuPlugin>()
                .As<ILicenseManager>()
                .InstancePerLifetimeScope();

            builder.RegisterType<LicenseService>().As<ILicenseService>().InstancePerLifetimeScope();

            builder.RegisterType<LicenseModelFactory>().As<ILicenseModelFactory>().InstancePerLifetimeScope();

            //object context
            builder.RegisterDataContext<LicenseObjectContext>("LicenseObjectContext");

            //override required repository with our custom context
            builder.RegisterType<Repository<License>>().As<IRepository<License>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("LicenseObjectContext"))
                .InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}