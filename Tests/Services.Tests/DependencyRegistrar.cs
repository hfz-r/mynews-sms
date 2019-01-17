using Autofac;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Infrastructure;

namespace Services.Tests
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<NullCache>().As<ICacheManager>().Named<ICacheManager>("cache_static").SingleInstance();
        }

        public int Order { get { return 0; } }
    }
}