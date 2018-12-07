using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Apps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Extensions
{
    public static class ServiceCollectionExt
    {
        public static IServiceCollection AddStartupTime(this IServiceCollection services)
        {
            services.AddSingleton<StartupTimeApp>();
            return services;
        }
    }
}
