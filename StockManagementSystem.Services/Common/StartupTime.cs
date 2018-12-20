using System;

namespace StockManagementSystem.Services.Common
{
    public class StartupTime : IStartupTime
    {
        public TimeSpan Uptime
        {
            get { return DateTime.Now - _startTime; }
        }

        private DateTime _startTime;

        public void Init()
        {
            _startTime = DateTime.Now;
        }
    }
}