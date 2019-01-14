using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Services.Locations
{
    public interface ILocationService
    {
        void DeleteShelfLocationFormat(ShelfLocationFormat shelfLocationFormat);
        Task<ShelfLocationFormat> GetShelfLocationFormatByIdAsync(int shelflocationFormatId);
        Task<ICollection<ShelfLocationFormat>> GetAllShelfLocationFormatsAsync();
        Task InsertShelfLocationFormat(ShelfLocationFormat shelfLocationFormat);
        void UpdateShelfLocationFormat(ShelfLocationFormat shelfLocationFormat);
    }
}