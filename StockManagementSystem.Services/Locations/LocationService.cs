using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManagementSystem.Services.Locations
{
    public class LocationService : ILocationService
    {
        private readonly IRepository<ShelfLocation> _shelfLocationRepository;
        private readonly IRepository<ShelfLocationFormat> _shelfLocationFormatRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Item> _itemRepository;

        public LocationService(
            IRepository<ShelfLocation> shelfLocationRepository,
            IRepository<ShelfLocationFormat> shelfLocationFormatRepository,
            IRepository<Store> storeRepository,
            IRepository<Item> itemRepository)
        {
            _shelfLocationRepository = shelfLocationRepository;
            _shelfLocationFormatRepository = shelfLocationFormatRepository;
            _storeRepository = storeRepository;
            _itemRepository = itemRepository;
        }
        
        public Task<ICollection<ShelfLocationFormat>> GetAllShelfLocationFormatsAsync()
        {
            var query = _shelfLocationFormatRepository.Table;

            query = query.OrderBy(c => c.Id);

            return Task.FromResult<ICollection<ShelfLocationFormat>>(new List<ShelfLocationFormat>(query.ToList()));
        }

        public virtual void UpdateShelfLocationFormat(ShelfLocationFormat shelfLocationFormat)
        {
            if (shelfLocationFormat == null)
                throw new ArgumentNullException(nameof(shelfLocationFormat));

            _shelfLocationFormatRepository.Update(shelfLocationFormat);
        }

        public virtual void DeleteShelfLocationFormat(ShelfLocationFormat shelfLocationFormat)
        {
            if (shelfLocationFormat == null)
                throw new ArgumentNullException(nameof(shelfLocationFormat));
            
            _shelfLocationFormatRepository.Delete(shelfLocationFormat);
        }

        public async Task InsertShelfLocationFormat(ShelfLocationFormat shelfLocationFormat)
        {
            if (shelfLocationFormat == null)
                throw new ArgumentNullException(nameof(shelfLocationFormat));

            await _shelfLocationFormatRepository.InsertAsync(shelfLocationFormat);
        }

        #region Identity

        public async Task<ShelfLocationFormat> GetShelfLocationFormatByIdAsync(int shelflocationFormatId)
        {
            if (shelflocationFormatId == 0)
                throw new ArgumentNullException(nameof(shelflocationFormatId));

            var shelflocationFormat = await _shelfLocationFormatRepository.Table.FirstOrDefaultAsync(u => u.Id == shelflocationFormatId);
            return shelflocationFormat;
        }

        #endregion
    }
}
