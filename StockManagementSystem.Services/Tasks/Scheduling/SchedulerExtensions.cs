using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Integrations;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Services.Tasks.Scheduling
{
    public static class SchedulerExtensions
    {
        public static void AddScheduledTasks(this IServiceCollection services)
        {
            //TODO: ability to run and stop manually
            //services.AddSingleton<IScheduledTask, ClearCacheTask>(); disabled
            //services.AddSingleton<IScheduledTask, ClearLogTask>(); disabled
            services.AddSingleton<IScheduledTask, KeepAliveTask>();
            services.AddSingleton<IScheduledTask, DeleteGuestsTask>();
            services.AddSingleton<IScheduledTask, DownloadMasterDataTask>();
        }

        public static IServiceCollection AddScheduler(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler)
        {
            return services.AddSingleton<IHostedService, SchedulerHostedService>(serviceProvider =>
            {
                var instance = new SchedulerHostedService(serviceProvider.GetServices<IScheduledTask>());
                instance.UnobservedTaskException += unobservedTaskExceptionHandler;
                return instance;
            });
        }
    }
}