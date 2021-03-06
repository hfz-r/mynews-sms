﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.Converters;
using StockManagementSystem.Api.Delta;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.Json.Maps;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.ModelBinders;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Api.Validators;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Infrastructure.DependencyManagement;

namespace StockManagementSystem.Api.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, DefaultConfig config)
        {
            builder.RegisterModelBinders();
            builder.RegisterServices();
            builder.RegisterByContext(typeFinder);
        }

        public int Order => 10;
    }

    internal static class DependencyHelper
    {
        public static void RegisterModelBinders(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ParametersModelBinder<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(JsonModelBinder<>)).InstancePerLifetimeScope();
        }

        public static void RegisterServices(this ContainerBuilder builder)
        {
            builder.RegisterType<ClientService>().As<IClientService>().InstancePerLifetimeScope();
            builder.RegisterType<UserApiService>().As<IUserApiService>().InstancePerLifetimeScope();
            builder.RegisterType<DeviceApiService>().As<IDeviceApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ItemApiService>().As<IItemApiService>().InstancePerLifetimeScope();
            builder.RegisterType<RoleApiService>().As<IRoleApiService>().InstancePerLifetimeScope();
            builder.RegisterType<PushNotificationApiService>().As<IPushNotificationApiService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<TransactionApiService>().As<ITransactionApiService>().InstancePerLifetimeScope();
            builder.RegisterType<TransporterTransactionApiService>().As<ITransporterTransactionApiService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ShelfLocationApiService>().As<IShelfLocationApiService>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(GenericApiService<,>)).As(typeof(IGenericApiService<,>))
                .InstancePerLifetimeScope();
            builder.RegisterType<LicenseApiService>().As<ILicenseApiService>().InstancePerLifetimeScope();

            builder.RegisterType<MappingHelper>().As<IMappingHelper>().InstancePerLifetimeScope();
            builder.RegisterType<UserRolesHelper>().As<IUserRolesHelper>().InstancePerLifetimeScope();
            builder.RegisterType<StoreMappingHelper>().As<IStoreMappingHelper>().InstancePerLifetimeScope();
            builder.RegisterType<JsonHelper>().As<IJsonHelper>().InstancePerLifetimeScope();
            builder.RegisterType<DtoHelper>().As<IDtoHelper>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigManagerHelper>().As<IConfigManagerHelper>().InstancePerLifetimeScope();

            builder.RegisterType<WebHookService>().As<IWebHookService>().SingleInstance();

            builder.RegisterType<JsonFieldsSerializer>().As<IJsonFieldsSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<FieldsValidator>().As<IFieldsValidator>().InstancePerLifetimeScope();

            builder.RegisterType<ObjectConverter>().As<IObjectConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ApiTypeConverter>().As<IApiTypeConverter>().InstancePerLifetimeScope();

            builder.RegisterType<ApiSettingModelFactory>().As<IApiSettingModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<UserFactory>().As<IFactory<User>>().InstancePerLifetimeScope();
            builder.RegisterType<DeviceFactory>().As<IFactory<Device>>().InstancePerLifetimeScope();
            builder.RegisterType<RoleFactory>().As<IFactory<Role>>().InstancePerLifetimeScope();
            builder.RegisterType<OrderLimitFactory>().As<IFactory<OrderLimit>>().InstancePerLifetimeScope();

            builder.RegisterType<JsonPropertyMapper>().As<IJsonPropertyMapper>().InstancePerLifetimeScope();

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            builder.RegisterType<Dictionary<string, object>>().SingleInstance();
        }

        public static void RegisterByContext(this ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //register automapper resolvers
            var resolvers = typeFinder.FindClassesOfType(typeof(IValueResolver<,,>)).ToList();
            foreach (var resolver in resolvers)
                builder.RegisterType(resolver).InstancePerLifetimeScope();
        }
    }
}