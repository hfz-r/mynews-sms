using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Data.Extensions;

namespace StockManagementSystem.Data
{
    public class SqlServerDataProvider : IDataProvider
    {
        public void InitializeDatabase()
        {
            var context = EngineContext.Current.Resolve<IDbContext>();

            var tableNamesToValidate = new List<string> {"Users", "Roles", "Device"};
            var existingTableNames = context.QueryFromSql<StringQueryType>(
                    "SELECT table_name AS Value FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'")
                .Select(stringValue => stringValue.Value).ToList();

            var createTables = !existingTableNames.Intersect(tableNamesToValidate, StringComparer.InvariantCultureIgnoreCase).Any();
            if (!createTables)
                return;

            var fileProvider = EngineContext.Current.Resolve<IFileProviderHelper>();

            //create tables
            context.ExecuteSqlScript(context.GenerateCreateScript());

            //create indexes
            context.ExecuteSqlScriptFromFile(fileProvider.MapPath(DataDefaults.SqlServerIndexesFilePath));

            //create stored procedures 
            context.ExecuteSqlScriptFromFile(fileProvider.MapPath(DataDefaults.SqlServerStoredProceduresFilePath));
        }

        public DbParameter GetParameter()
        {
            return new SqlParameter();
        }

        public bool BackupSupported => true;

        public int SupportedLengthOfBinaryHash => 8000;
    }
}