using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Logging
{
    public interface ILogger
    {
        Task ClearLog();

        Task DeleteLog(Log log);

        Task DeleteLogs(IList<Log> logs);

        Task<IPagedList<Log>> GetAllLogs(DateTime? fromUtc = null, DateTime? toUtc = null, string message = "", LogLevel? logLevel = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<Log> GetLogById(int logId);

        Task<IList<Log>> GetLogByIds(int[] logIds);

        Task<Log> InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null);

        bool IsEnabled(LogLevel level);

        void Warning(string message, Exception exception = null, User user = null);

        void Error(string message, Exception exception = null, User user = null);

        void Information(string message, Exception exception = null, User user = null);
    }
}