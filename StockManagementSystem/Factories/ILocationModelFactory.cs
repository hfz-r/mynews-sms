using System.Threading.Tasks;
using StockManagementSystem.Models.Locations;

namespace StockManagementSystem.Factories
{
    public interface ILocationModelFactory
    {
        Task<LocationListModel> PrepareShelfLocationFormatListModel(LocationSearchModel searchModel);
        Task<LocationSearchModel> PrepareShelfLocationFormatSearchModel(LocationSearchModel searchModel);
    }
}