using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Tasks.Scheduling;

namespace StockManagementSystem.Web.Infrastructure
{
    public class SchedulerStartup : IBaseStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            if (!DataSettingsManager.DatabaseIsInstalled)
                return;

            services.AddScheduledTasks();
            services.AddScheduler((sender, args) =>
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error("One or more scheduled tasks failed to complete.", args.Exception);
                args.SetObserved();
            });
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        //should be loaded last or after mvc
        public int Order => 1500;
    }
}