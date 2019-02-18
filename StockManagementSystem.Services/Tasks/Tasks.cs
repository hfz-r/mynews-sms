using System;
using System.Linq;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Tasks;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Logging;

namespace StockManagementSystem.Services.Tasks
{
    public class Tasks
    {
        private bool? _enabled;

        public Tasks(ScheduleTask task)
        {
            ScheduleTask = task;
        }

        #region Utilities

        /// <summary>
        /// Initialize and execute task
        /// </summary>
        private void ExecuteTask()
        {
            var scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();

            if (!Enabled)
                return;

            var type = Type.GetType(ScheduleTask.Type) ??
                       //ensure that it works fine when only the type name is specified (do not require fully qualified names)
                       AppDomain.CurrentDomain.GetAssemblies()
                           .Select(a => a.GetType(ScheduleTask.Type))
                           .FirstOrDefault(t => t != null);
            if (type == null)
                throw new Exception($"Schedule task ({ScheduleTask.Type}) cannot by instantiated");

            object instance = null;
            try
            {
                instance = EngineContext.Current.Resolve(type);
            }
            catch
            {
                //try resolve
            }

            if (instance == null)
            {
                //not resolved
                instance = EngineContext.Current.ResolveUnregistered(type);
            }

            if (!(instance is IScheduleTask task))
                return;

            ScheduleTask.LastStartUtc = DateTime.UtcNow;
            //update appropriate datetime properties
            scheduleTaskService.UpdateTaskAsync(ScheduleTask).GetAwaiter().GetResult();
            task.Execute();
            ScheduleTask.LastEndUtc = ScheduleTask.LastSuccessUtc = DateTime.UtcNow;
            //update appropriate datetime properties
            scheduleTaskService.UpdateTaskAsync(ScheduleTask).GetAwaiter().GetResult();
        }

        protected virtual bool IsTaskAlreadyRunning(ScheduleTask scheduleTask)
        {
            //task run for the first time
            if (!scheduleTask.LastStartUtc.HasValue && !scheduleTask.LastEndUtc.HasValue)
                return false;

            var lastStartUtc = scheduleTask.LastStartUtc ?? DateTime.UtcNow;

            //task already finished
            if (scheduleTask.LastEndUtc.HasValue && lastStartUtc < scheduleTask.LastEndUtc)
                return false;

            //task wasn't finished last time
            if (lastStartUtc.AddSeconds(scheduleTask.Seconds) <= DateTime.UtcNow)
                return false;

            return true;
        }

        #endregion

        public void Execute(bool throwException = false, bool ensureRunOncePerPeriod = true)
        {
            if (ScheduleTask == null || !Enabled)
                return;

            if (ensureRunOncePerPeriod)
            {
                //task already running
                if (IsTaskAlreadyRunning(ScheduleTask))
                    return;

                //validation (so nobody else can invoke this method when he wants)
                if (ScheduleTask.LastStartUtc.HasValue && (DateTime.UtcNow - ScheduleTask.LastStartUtc).Value.TotalSeconds < ScheduleTask.Seconds)
                    //too early
                    return;
            }

            try
            {
                //get expiration time
                var expirationInSeconds = Math.Min(ScheduleTask.Seconds, 300) - 1;
                var expiration = TimeSpan.FromSeconds(expirationInSeconds);

                //execute task with lock
                var locker = EngineContext.Current.Resolve<ILocker>();
                locker.PerformActionWithLock(ScheduleTask.Type, expiration, ExecuteTask);
            }
            catch (Exception exc)
            {
                var scheduleTaskService = EngineContext.Current.Resolve<IScheduleTaskService>();

                ScheduleTask.Enabled = !ScheduleTask.StopOnError;
                ScheduleTask.LastEndUtc = DateTime.UtcNow;
                scheduleTaskService.UpdateTaskAsync(ScheduleTask).GetAwaiter().GetResult();

                //log error
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error($"Error while running the '{ScheduleTask.Name}' schedule task. {exc.Message}", exc);
                if (throwException)
                    throw;
            }
        }

        #region Properties

        public ScheduleTask ScheduleTask { get; }

        public bool Enabled
        {
            get
            {
                if (!_enabled.HasValue)
                    _enabled = ScheduleTask?.Enabled;

                return _enabled.HasValue && _enabled.Value;
            }

            set => _enabled = value;
        }

        #endregion

    }
}