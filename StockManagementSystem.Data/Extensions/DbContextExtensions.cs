using System;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;

namespace StockManagementSystem.Data.Extensions
{
    public static class DbContextExtensions
    {
        private static readonly ConcurrentDictionary<string, string> tableNames = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Get table name of entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="context">Database context</param>
        /// <returns>Table name</returns>
        public static string GetTableName<TEntity>(this IDbContext context) where TEntity : BaseEntity
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            //try to get the EF database context
            if (!(context is DbContext dbContext))
                throw new InvalidOperationException("Context does not support operation");

            var entityTypeFullName = typeof(TEntity).FullName;
            if (!tableNames.ContainsKey(entityTypeFullName))
            {
                //get entity type
                var entityType = dbContext.Model.FindRuntimeEntityType(typeof(TEntity));

                //get the name of the table to which the entity type is mapped
                tableNames.TryAdd(entityTypeFullName, entityType.Relational().TableName);
            }

            tableNames.TryGetValue(entityTypeFullName, out var tableName);

            return tableName;
        }
    }
}