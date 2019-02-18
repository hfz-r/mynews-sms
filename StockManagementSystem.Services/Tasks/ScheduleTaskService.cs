using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Tasks;

namespace StockManagementSystem.Services.Tasks
{
    public class ScheduleTaskService : IScheduleTaskService
    {
        private readonly IRepository<ScheduleTask> _taskRepository;

        public ScheduleTaskService(IRepository<ScheduleTask> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task DeleteTaskAsync(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _taskRepository.DeleteAsync(task);
        }

        public async Task<ScheduleTask> GetTaskByIdAsync(int taskId)
        {
            if (taskId == 0)
                return null;

            return await _taskRepository.GetByIdAsync(taskId);
        }

        public async Task<ScheduleTask> GetTaskByTypeAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;

            var query = _taskRepository.Table;
            query = query.Where(st => st.Type == type);
            query = query.OrderByDescending(t => t.Id);

            var task = await query.FirstOrDefaultAsync();
            return task;
        }

        public async Task<IList<ScheduleTask>> GetAllTasksAsync(bool showHidden = false)
        {
            var query = _taskRepository.Table;
            if (!showHidden)
            {
                query = query.Where(t => t.Enabled);
            }

            query = query.OrderByDescending(t => t.Seconds);

            var tasks = await query.ToListAsync();
            return tasks;
        }

        public async Task InsertTaskAsync(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _taskRepository.InsertAsync(task);
        }

        public async Task UpdateTaskAsync(ScheduleTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            await _taskRepository.UpdateAsync(task);
        }
    }
}