using Autofac;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Infrastructure.DependencyManagement;
using StockManagementSystem.Factories;
using StockManagementSystem.Web.Factories;

namespace StockManagementSystem.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, DefaultConfig config)
        {
            //common factories
            builder.RegisterType<AclSupportedModelFactory>().As<IAclSupportedModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<TenantMappingSupportedModelFactory>().As<ITenantMappingSupportedModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<StoreMappingSupportedModelFactory>().As<IStoreMappingSupportedModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<AppliedStoreSupportedModelFactory>().As<IAppliedStoreSupportedModelFactory>().InstancePerLifetimeScope();

            //admin factories
            builder.RegisterType<ReportModelFactory>().As<IReportModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ActivityLogModelFactory>().As<IActivityLogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<BaseModelFactory>().As<IBaseModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<UserModelFactory>().As<IUserModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<UserAccountModelFactory>().As<IUserAccountModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<RoleModelFactory>().As<IRoleModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<StoreModelFactory>().As<IStoreModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<CommonModelFactory>().As<ICommonModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DeviceModelFactory>().As<IDeviceModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<OrderLimitModelFactory>().As<IOrderLimitModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SettingModelFactory>().As<ISettingModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ReplenishmentModelFactory>().As<IReplenishmentModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<PushNotificationModelFactory>().As<IPushNotificationModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ManagementModelFactory>().As<IManagementModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<LocationModelFactory>().As<ILocationModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<FormatSettingModelFactory>().As<IFormatSettingModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SecurityModelFactory>().As<ISecurityModelFactory>().InstancePerLifetimeScope();
        }

        public int Order => 2;
    }
}