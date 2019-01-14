using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Locations;
using StockManagementSystem.Services.Locations;
using StockManagementSystem.Web.Extensions;
using StockManagementSystem.Web.Kendoui.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Factories
{
    public class LocationModelFactory : ILocationModelFactory
    {
        private readonly ILocationService _locationService;

        public LocationModelFactory(
            ILocationService locationService
            )
        {
            _locationService = locationService;
        }

        public async Task<LocationSearchModel> PrepareShelfLocationFormatSearchModel(LocationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);
        }

        public async Task<LocationListModel> PrepareShelfLocationFormatListModel(LocationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var shelfLocationFormats = await _locationService.GetAllShelfLocationFormatsAsync();

            if (shelfLocationFormats == null)
                throw new ArgumentNullException(nameof(shelfLocationFormats));

            var model = new LocationListModel
            {
                //Data = shelfLocationFormats.Select(shelfLocationFormat =>
                //{
                //    var shelfLocationFormatsModel = shelfLocationFormat.ToModel<LocationModel>();
                //    return shelfLocationFormatsModel;
                //}),
                Data = shelfLocationFormats.PaginationByRequestModel(searchModel).Select(shelfLocationFormat =>
                {
                    var shelfLocationFormatsModel = shelfLocationFormat.ToModel<LocationModel>();
                    return shelfLocationFormatsModel;
                }),
                Total = shelfLocationFormats.Count
            };

            // sort
            if (searchModel.Sort != null && searchModel.Sort.Any())
            {
                foreach (var s in searchModel.Sort)
                {
                    model.Data = await model.Data.Sort(s.Field, s.Dir);
                }
            }

            // filter
            if (searchModel.Filter != null && searchModel.Filter.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }
    }
}
