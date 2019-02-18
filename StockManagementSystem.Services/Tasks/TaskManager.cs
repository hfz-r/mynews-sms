using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Services.Tasks
{
    public class TaskManager
    {
        private readonly List<TaskThread> _taskThreads = new List<TaskThread>();

        private TaskManager()
        {
        }

        /// <summary>
        /// Initializes the task manager
        /// </summary>
        public void Initialize()
        {
            _taskThreads.Clear();

            var taskService = EngineContext.Current.Resolve<IScheduleTaskService>();

            var scheduleTasks = taskService.GetAllTasksAsync().GetAwaiter().GetResult().OrderBy(x => x.Seconds).ToList();

            foreach (var scheduleTask in scheduleTasks)
            {
                var taskThread = new TaskThread
                {
                    Seconds = scheduleTask.Seconds
                };

                if (scheduleTask.LastStartUtc.HasValue)
                {
                    //seconds left since the last start
                    var secondsLeft = (DateTime.UtcNow - scheduleTask.LastStartUtc).Value.TotalSeconds;

                    if (secondsLeft >= scheduleTask.Seconds)
                        //run now (immediately)
                        taskThread.InitSeconds = 0;
                    else
                        //calculate start time
                        //and round it (so "ensureRunOncePerPeriod" parameter was fine)
                        taskThread.InitSeconds = (int)(scheduleTask.Seconds - secondsLeft) + 1;
                }
                else
                {
                    //first start of a task
                    taskThread.InitSeconds = scheduleTask.Seconds;
                }

                taskThread.AddTask(scheduleTask);
                _taskThreads.Add(taskThread);
            }
        }

        /// <summary>
        /// Starts the task manager
        /// </summary>
        public void Start()
        {
            foreach (var taskThread in _taskThreads)
            {
                taskThread.InitTimer();
            }
        }

        /// <summary>
        /// Stops the task manager
        /// </summary>
        public void Stop()
        {
            foreach (var taskThread in _taskThreads)
            {
                taskThread.Dispose();
            }
        }

        #region Properties

        /// <summary>
        /// Gets the task manger instance
        /// </summary>
        public static TaskManager Instance { get; } = new TaskManager();

        /// <summary>
        /// Gets a list of task threads of this task manager
        /// </summary>
        public IList<TaskThread> TaskThreads => new ReadOnlyCollection<TaskThread>(_taskThreads);

        #endregion
    }
}