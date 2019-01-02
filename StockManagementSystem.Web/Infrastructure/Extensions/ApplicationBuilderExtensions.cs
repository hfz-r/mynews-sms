using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Builder;
using StockManagementSystem.Services.Common;

namespace StockManagementSystem.Web.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }

        public static void UseDefaultExceptionHandler(this IApplicationBuilder application)
        {
            var hostingEnvironment = EngineContext.Current.Resolve<IHostingEnvironment>();
            if (hostingEnvironment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
                application.UseDatabaseErrorPage();
            }
            else
            {
                application.UseExceptionHandler("/Home/Error");
            }

            // TODO: log errors
            application.UseExceptionHandler(handler =>
            {
                handler.Run(context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return Task.CompletedTask;

                    try
                    {
                        var currentUser = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;
                        EngineContext.Current.Resolve<ILogger>().LogError(exception.Message, exception, currentUser);
                    }
                    finally
                    {
                        //rethrow the exception to show the error page
                        ExceptionDispatchInfo.Throw(exception);
                    }

                    return Task.CompletedTask;
                });
            });
        }

        public static void UseBadRequestResult(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(context =>
            {
                //handle 404 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    // TODO: log erros
                }

                return Task.CompletedTask;
            });
        }

        public static void UseDefaultStaticFiles(this IApplicationBuilder application)
        {
            application.UseStaticFiles();

            var hostingEnvironment = EngineContext.Current.Resolve<IHostingEnvironment>();
            var cachePeriod = hostingEnvironment.IsDevelopment() ? "600" : "604800";
            application.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                }
            });
        }

        public static void UseDefaultMvc(this IApplicationBuilder application)
        {
            application.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public static void UseStartupTime(this IApplicationBuilder application)
        {
            var startupTime = EngineContext.Current.Resolve<IStartupTime>();
            startupTime.Init();
        }
    }
}