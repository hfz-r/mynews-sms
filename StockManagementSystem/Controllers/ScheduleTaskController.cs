using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Services.Tasks;

namespace StockManagementSystem.Controllers
{
    public class ScheduleTaskController : Controller
    {
        private readonly IScheduleTaskService _scheduleTaskService;

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService)
        {
            _scheduleTaskService = scheduleTaskService;
        }

        [HttpPost]
        public async Task<IActionResult> RunTask(string taskType)
        {
            var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(taskType);
            if (scheduleTask == null)
                //schedule task cannot be loaded
                return NoContent();

            var task = new Tasks(scheduleTask);
            task.Execute();

            return NoContent();
        }
    }
}