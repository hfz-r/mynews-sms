﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StockManagementSystem.Core;

namespace StockManagementSystem.Data.Extensions
{
    public static class DbContextExtensions
    {
        private static string databaseName;
        private static readonly ConcurrentDictionary<string, string> tableNames = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, IEnumerable<(string, int?)>> columnsMaxLength = new ConcurrentDictionary<string, IEnumerable<(string, int?)>>();
        private static readonly ConcurrentDictionary<string, IEnumerable<(string, decimal?)>> decimalColumnsMaxValue = new ConcurrentDictionary<string, IEnumerable<(string, decimal?)>>();

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

        /// <summary>
        /// Gets the maximum lengths of data that is allowed for the entity properties
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="context">Database context</param>
        /// <returns>Collection of name - max length pairs</returns>
        public static IEnumerable<(string Name, int? MaxLength)> GetColumnsMaxLength<TEntity>(this IDbContext context)
            where TEntity : BaseEntity
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            //try to get the EF database context
            if (!(context is DbContext dbContext))
                throw new InvalidOperationException("Context does not support operation");

            var entityTypeFullName = typeof(TEntity).FullName;
            if (!columnsMaxLength.ContainsKey(entityTypeFullName))
            {
                //get entity type
                var entityType = dbContext.Model.FindEntityType(typeof(TEntity));

                //get property name - max length pairs
                columnsMaxLength.TryAdd(entityTypeFullName,
                    entityType.GetProperties().Select(property => (property.Name, property.GetMaxLength())));
            }

            columnsMaxLength.TryGetValue(entityTypeFullName, out var result);

            return result;
        }

        /// <summary>
        /// Get maximum decimal values
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="context">Database context</param>
        /// <returns>Collection of name - max decimal value pairs</returns>
        public static IEnumerable<(string Name, decimal? MaxValue)> GetDecimalColumnsMaxValue<TEntity>(this IDbContext context)
            where TEntity : BaseEntity
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            //try to get the EF database context
            if (!(context is DbContext dbContext))
                throw new InvalidOperationException("Context does not support operation");

            var entityTypeFullName = typeof(TEntity).FullName;
            if (!decimalColumnsMaxValue.ContainsKey(entityTypeFullName))
            {
                //get entity type
                var entityType = dbContext.Model.FindEntityType(typeof(TEntity));

                //get entity decimal properties
                var properties = entityType.GetProperties().Where(property => property.ClrType == typeof(decimal));

                //return property name - max decimal value pairs
                decimalColumnsMaxValue.TryAdd(entityTypeFullName, properties.Select(property =>
                {
                    var mapping = new RelationalTypeMappingInfo(property);
                    if (!mapping.Precision.HasValue || !mapping.Scale.HasValue)
                        return (property.Name, null);

                    return (property.Name, new decimal?((decimal)Math.Pow(10, mapping.Precision.Value - mapping.Scale.Value)));
                }));
            }

            decimalColumnsMaxValue.TryGetValue(entityTypeFullName, out var result);

            return result;
        }

        /// <summary>
        /// Get database name
        /// </summary>
        /// <param name="context">Database context</param>
        /// <returns>Database name</returns>
        public static string DbName(this IDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            //try to get the EF database context
            if (!(context is DbContext dbContext))
                throw new InvalidOperationException("Context does not support operation");

            if (!string.IsNullOrEmpty(databaseName))
                return databaseName;

            //get database connection
            var dbConnection = dbContext.Database.GetDbConnection();

            //return the database name
            databaseName = dbConnection.Database;

            return databaseName;
        }
    }
}