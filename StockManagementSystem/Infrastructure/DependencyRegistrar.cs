using Autofac;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Factories;

namespace StockManagementSystem.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<RoleModelFactory>().As<IRoleModelFactory>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}