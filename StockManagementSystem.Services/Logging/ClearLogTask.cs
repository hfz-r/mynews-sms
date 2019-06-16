using System.Threading;
using System.Threading.Tasks;
using StockManagementSystem.Services.Tasks.Scheduling;

namespace StockManagementSystem.Services.Logging
{
    /// <summary>
    /// Represents a task to clear [Log] table
    /// </summary>
    public class ClearLogTask : IScheduledTask
    {
        private readonly ILogger _logger;

        public ClearLogTask(ILogger logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.ClearLog();

            return Task.CompletedTask;
        }

        public string Schedule => "0 */2 * * *";

        public bool Enabled => false;
    }
}