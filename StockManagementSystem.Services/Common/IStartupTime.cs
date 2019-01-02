using System;

namespace StockManagementSystem.Services.Common
{
    public interface IStartupTime
    {
        TimeSpan Uptime { get; }

        void Init();
    }
}