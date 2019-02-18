using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.IdentityServer.Middlewares;
using StockManagementSystem.Core.Infrastructure;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace StockManagementSystem.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ApplyIdentityServerMigrations(this IApplicationBuilder application)
        {
            using (var serviceScope = application.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                /* the database.Migrate command will apply all pending migrations and will 
                 * create the database if it is not created already. */
                var persistedGrantContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                persistedGrantContext.Database.Migrate();

                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationContext.Database.Migrate();
            }
        }

        public static void ApplyApiSeedData(this IApplicationBuilder application)
        {
            using (var serviceScope = application.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!configurationContext.ApiResources.Any())
                {
                    /* In the simple case an API has exactly one scope. 
                     * But there are cases where you might want to sub-divide the functionality of an API, 
                     * and give different clients access to different parts. */
                    configurationContext.ApiResources.Add(new ApiResource()
                    {
                        Enabled = true,
                        Scopes = new List<ApiScope>()
                        {
                            new ApiScope()
                            {
                                Name = "sms_api",
                                DisplayName = "sms_api"
                            }
                        },
                        Name = "sms_api"
                    });

                    configurationContext.SaveChanges();

                    TryRunUpgradeScript(configurationContext).GetAwaiter().GetResult();
                }
            }
        }

        public static void UseUrlRewrite(this IApplicationBuilder application)
        {
            var options = new RewriteOptions()
                .AddRewrite("oauth/(.*)", "connect/$1", true)
                .AddRewrite("api/token", "connect/token", true);

            application.UseRewriter(options);
        }

        public static void UseIdentityServerEntityFramework(this IApplicationBuilder application)
        {
            //angular client; directly calls the oauth endpoint
            //application.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            // use scope parameters middleware
            application.UseMiddleware<ScopeParametersMiddleware>();

            application.UseMiddleware<BaseUrlMiddleware>();
            application.ConfigureCors();

            // application.UseAuthentication(); no need since IdentityServer included this  
            application.UseMiddleware<IdentityServerMiddleware>();
        }

        #region Private methods

        private static async Task TryRunUpgradeScript(ConfigurationDbContext configurationContext)
        {
            try
            {
                /* If there are no api resources we can assume that this is the first start after the upgrade 
                 * and run the upgrade script. */
                var upgradeScript = await LoadUpgradeScript();
                await configurationContext.Database.ExecuteSqlCommandAsync(upgradeScript);

                // All client secrets must be hashed otherwise the identity server validation will fail.
                var allClients = await configurationContext.Clients
                    .Include(client => client.ClientSecrets).ToListAsync();
                foreach (var client in allClients)
                {
                    foreach (var clientSecret in client.ClientSecrets)
                    {
                        clientSecret.Value = clientSecret.Value.Sha256();
                    }

                    client.AccessTokenLifetime = Configurations.DefaultAccessTokenExpiration;
                    client.AbsoluteRefreshTokenLifetime = Configurations.DefaultRefreshTokenExpiration;
                }

                await configurationContext.SaveChangesAsync();
            }
            catch
            {
                // Do nothing
            }
        }

        private static async Task<string> LoadUpgradeScript()
        {
            var fileProvider = EngineContext.Current.Resolve<FileProviderHelper>();
            string path = fileProvider.MapPath($"~/Plugins/StockManagementSystem.Api/{ApiSettingsDefaults.ApiUpgradeScript}");
            string script = await File.ReadAllTextAsync(path);

            return script;
        }

        #endregion
    }
}