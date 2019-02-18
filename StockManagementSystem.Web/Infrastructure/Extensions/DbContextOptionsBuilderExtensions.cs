using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Web.Infrastructure.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static void UseSqlServerWithLazyLoading(this DbContextOptionsBuilder optionsBuilder,
            IServiceCollection services)
        {
            var defaultConfig = services.BuildServiceProvider().GetRequiredService<DefaultConfig>();

            var dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;

            var dbContextOptionsBuilder = optionsBuilder.UseLazyLoadingProxies();

            if (defaultConfig.UseRowNumberForPaging)
                dbContextOptionsBuilder.UseSqlServer(dataSettings.DataConnectionString,
                    option => option.UseRowNumberForPaging());
            else
                dbContextOptionsBuilder.UseSqlServer(dataSettings.DataConnectionString);

            //.UseSqlServer(dataSettings.DataConnectionString, builder => builder.MigrationsAssembly("StockManagementSystem"));
        }
    }
}