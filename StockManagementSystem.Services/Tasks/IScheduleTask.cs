namespace StockManagementSystem.Services.Tasks
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public interface IScheduleTask
    {
        void Execute();
    }
}