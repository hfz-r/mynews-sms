using StockManagementSystem.Services.Tasks;

namespace StockManagementSystem.Services.Logging
{
    /// <summary>
    /// Represents a task to clear [Log] table
    /// </summary>
    public class ClearLogTask : IScheduleTask
    {
        private readonly ILogger _logger;

        public ClearLogTask(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute()
        {
            _logger.ClearLog();
        }
    }
}