using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Data.Infrastructure
{
    /// <summary>
    /// Represent helper for SqlClient providers
    /// </summary>
    public static class SqlHelper
    {
        #region Private methods

        /// <summary>
        /// Get connection string setup on appsettings.json which currently available for HHT_POS 
        /// </summary>
        /// <returns></returns>
        private static string GetConnectionString()
        {
            var configuration = EngineContext.Current.Resolve<IConfiguration>();
            var connStr = configuration.GetConnectionString("HQ");

            return connStr;
        }

        #endregion

        public static async Task<int> ExecuteNonQuery(SqlConnection conn, string cmdText, SqlParameter[] cmdParams)
        {
            SqlCommand cmd = conn.CreateCommand();
            await PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, cmdParams);
            int val = await cmd.ExecuteNonQueryAsync();
            cmd.Parameters.Clear();
            return val;
        }

        public static async Task<int> ExecuteNonQuery(SqlConnection conn, CommandType cmdType, string cmdText,
            SqlParameter[] cmdParams)
        {
            SqlCommand cmd = conn.CreateCommand();
            using (conn)
            {
                await PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParams);
                int val = await cmd.ExecuteNonQueryAsync();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static async Task<SqlDataReader> ExecuteReader(SqlConnection conn, CommandType cmdType, string cmdText,
            SqlParameter[] cmdParams)
        {
            SqlCommand cmd = conn.CreateCommand();
            await PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParams);
            var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            return rdr;
        }

        public static DataTable ExecuteDataTable(SqlConnection conn, CommandType cmdType, string cmdText,
            SqlParameter[] cmdParams)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmdText, conn);
            da.Fill(dt);
            return dt;
        }

        public static async Task<object> ExecuteScalar(SqlConnection conn, CommandType cmdType, string cmdText,
            SqlParameter[] cmdParams)
        {
            SqlCommand cmd = conn.CreateCommand();
            await PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParams);
            object val = await cmd.ExecuteScalarAsync();
            cmd.Parameters.Clear();
            return val;
        }

        private static async Task PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans,
            CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(cmd, commandParameters);
            }
        }

        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (var p in commandParameters)
            {
                //check for derived output value with no value assigned
                if (p.Direction == ParameterDirection.InputOutput && p.Value == null)
                {
                    p.Value = DBNull.Value;
                }
                command.Parameters.Add(p);
            }
        }

        public static string ConnectionString { get; } = GetConnectionString();
    }
}