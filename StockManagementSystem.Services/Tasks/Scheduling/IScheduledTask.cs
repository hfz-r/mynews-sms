using System.Threading;
using System.Threading.Tasks;

namespace StockManagementSystem.Services.Tasks.Scheduling
{
    public interface IScheduledTask
    {
        string Schedule { get; }

        bool Enabled { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}