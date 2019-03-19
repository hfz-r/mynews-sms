using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Api.Helpers
{
    public interface IConfigManagerHelper
    {
        DataSettings DataSettings { get; }

        void AddBindingRedirects();

        void AddConnectionString();
    }
}