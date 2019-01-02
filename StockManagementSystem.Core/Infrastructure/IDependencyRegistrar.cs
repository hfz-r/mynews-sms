using Autofac;

namespace StockManagementSystem.Core.Infrastructure
{
    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);

        int Order { get; }
    }
}