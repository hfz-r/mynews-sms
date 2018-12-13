using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Apps;

namespace StockManagementSystem.Extensions
{
    public static class ApplicationBuilderExt
    {
        public static IApplicationBuilder UseStartupTime(this IApplicationBuilder builder)
        {
            var app = builder.ApplicationServices.GetRequiredService<StartupTimeApp>();
            app.Init();
            return builder;
        }
    }
}
