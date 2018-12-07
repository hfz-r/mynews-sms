using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Apps
{
    public class StartupTimeApp
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
