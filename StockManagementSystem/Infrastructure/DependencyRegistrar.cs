﻿using Autofac;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Factories;
using StockManagementSystem.Web.Factories;

namespace StockManagementSystem.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<UserModelFactory>().As<IUserModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<RoleModelFactory>().As<IRoleModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<DeviceModelFactory>().As<IDeviceModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<OrderLimitModelFactory>().As<IOrderLimitModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<LocationModelFactory>().As<ILocationModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SecurityModelFactory>().As<ISecurityModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<AclSupportedModelFactory>().As<IAclSupportedModelFactory>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}