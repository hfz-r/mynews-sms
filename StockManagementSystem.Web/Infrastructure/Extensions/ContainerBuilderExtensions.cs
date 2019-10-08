using System;
using Autofac;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Data;

namespace StockManagementSystem.Web.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of Autofac ContainerBuilder
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        public static void RegisterDataContext<TContext>(this ContainerBuilder builder, string contextName)
            where TContext : DbContext, IDbContext
        {
            builder.Register(context =>
                    (IDbContext) Activator.CreateInstance(typeof(TContext),
                        context.Resolve<DbContextOptions<TContext>>()))
                .Named<IDbContext>(contextName).InstancePerLifetimeScope();
        }
    }
}