using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Data;
using StockManagementSystem.Data.Extensions;

namespace StockManagementSystem.Services.Logging
{
    public class DefaultLogger : ILogger
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Log> _logRepository;
        private readonly IWebHelper _webHelper;

        public DefaultLogger(IDbContext dbContext, IRepository<Log> logRepository, IWebHelper webHelper)
        {
            _dbContext = dbContext;
            _logRepository = logRepository;
            _webHelper = webHelper;
        }

        public bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return false;
                default:
                    return true;
            }
        }

        public async Task DeleteLog(Log log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            await _logRepository.DeleteAsync(log);
        }

        public async Task DeleteLogs(IList<Log> logs)
        {
            if (logs == null)
                throw new ArgumentNullException(nameof(logs));

            await _logRepository.DeleteAsync(logs);
        }

        public void ClearLog()
        {
            var logTableName = _dbContext.GetTableName<Log>();
            _dbContext.ExecuteSqlCommand($"TRUNCATE TABLE [{logTableName}]");
        }

        public Task<IPagedList<Log>> GetAllLogs(
            DateTime? fromUtc = null, 
            DateTime? toUtc = null,
            string message = "", 
            LogLevel? logLevel = null,
            int pageIndex = 0, 
            int pageSize = int.MaxValue)
        {
            var query = _logRepository.Table;

            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);

            if (logLevel.HasValue)
            {
                var logLevelId = (int)logLevel.Value;
                query = query.Where(l => logLevelId == l.LogLevelId);
            }

            if (!string.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));

            query = query.OrderByDescending(l => l.CreatedOnUtc);

            return Task.FromResult<IPagedList<Log>>(new PagedList<Log>(query, pageIndex, pageSize));
        }

        public async Task<Log> GetLogById(int logId)
        {
            if (logId == 0)
                return null;

            return await _logRepository.GetByIdAsync(logId);
        }

        public async Task<IList<Log>> GetLogByIds(int[] logIds)
        {
            if (logIds == null || logIds.Length == 0)
                return new List<Log>();

            var query = from l in _logRepository.Table
                where logIds.Contains(l.Id)
                select l;
            var logItems = await query.ToListAsync();

            var sortedLogItems = new List<Log>();
            foreach (var id in logIds)
            {
                var log = logItems.Find(x => x.Id == id);
                if (log != null)
                    sortedLogItems.Add(log);
            }

            return sortedLogItems;
        }

        public async Task<Log> InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null)
        {
            var log = new Log
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                IpAddress = _webHelper.GetCurrentIpAddress(),
                User = user,
                PageUrl = _webHelper.GetThisPageUrl(true),
                ReferrerUrl = _webHelper.GetUrlReferrer(),
                CreatedOnUtc = DateTime.UtcNow
            };

            await _logRepository.InsertAsync(log);

            return log;
        }

        public void Information(string message, Exception exception = null, User user = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Information))
                InsertLog(LogLevel.Information, message, exception?.ToString() ?? string.Empty, user);
        }

        public void Warning(string message, Exception exception = null, User user = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Warning))
                InsertLog(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, user);
        }

        public void Error(string message, Exception exception = null, User user = null)
        {
            //don't log thread abort exception
            if (exception is System.Threading.ThreadAbortException)
                return;

            if (IsEnabled(LogLevel.Error))
                InsertLog(LogLevel.Error, message, exception?.ToString() ?? string.Empty, user);
        }

        #region Synchronous wrapper

        public Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null)
        {
            return InsertLogAsync(logLevel, shortMessage, fullMessage, user).GetAwaiter().GetResult();
        }

        #endregion
    }
}