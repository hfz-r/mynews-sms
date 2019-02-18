using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Tasks;

namespace StockManagementSystem.Services.Tasks
{
    public interface IScheduleTaskService
    {
        Task DeleteTaskAsync(ScheduleTask task);

        Task<IList<ScheduleTask>> GetAllTasksAsync(bool showHidden = false);

        Task<ScheduleTask> GetTaskByIdAsync(int taskId);

        Task<ScheduleTask> GetTaskByTypeAsync(string type);

        Task InsertTaskAsync(ScheduleTask task);

        Task UpdateTaskAsync(ScheduleTask task);
    }
}