using Autofac;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Infrastructure.DependencyManagement;

namespace Services.Tests
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, DefaultConfig config)
        {
            builder.RegisterType<NullCache>().As<ICacheManager>().Named<ICacheManager>("cache_static").SingleInstance();
        }

        public int Order => 0;
    }
}