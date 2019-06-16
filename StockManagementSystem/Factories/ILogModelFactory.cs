using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Models.Logging;

namespace StockManagementSystem.Factories
{
    public interface ILogModelFactory
    {
        Task<LogSearchModel> PrepareLogSearchModel(LogSearchModel searchModel);

        Task<LogListModel> PrepareLogListModel(LogSearchModel searchModel);

        Task<LogModel> PrepareLogModel(LogModel model, Log log, bool excludeProperties = false);
    }
}