using Autofac;
using StockManagementSystem.Core.Configuration;

namespace StockManagementSystem.Core.Infrastructure.DependencyManagement
{
    public interface IDependencyRegistrar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder, DefaultConfig config);

        int Order { get; }
    }
}