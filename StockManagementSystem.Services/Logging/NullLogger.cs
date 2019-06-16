using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Logging
{
    public class NullLogger : ILogger
    {
        public void ClearLog()
        {
        }

        public Task DeleteLog(Log log)
        {
            return Task.CompletedTask;
        }

        public Task DeleteLogs(IList<Log> logs)
        {
            return Task.CompletedTask;
        }

        public void Error(string message, Exception exception = null, User user = null)
        {
        }

        public Task<IPagedList<Log>> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null, string message = "", LogLevel? logLevel = null,
            int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            return Task.FromResult<IPagedList<Log>>(new PagedList<Log>(new List<Log>(), pageIndex, pageSize));
        }

        public Task<Log> GetLogById(int logId)
        {
            return null;
        }

        public Task<IList<Log>> GetLogByIds(int[] logIds)
        {
            return Task.FromResult<IList<Log>>(new List<Log>());
        }

        public void Information(string message, Exception exception = null, User user = null)
        {
        }

        public Task<Log> InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null)
        {
            return null;
        }

        public bool IsEnabled(LogLevel level)
        {
            return false;
        }

        public void Warning(string message, Exception exception = null, User user = null)
        {
        }
    }
}