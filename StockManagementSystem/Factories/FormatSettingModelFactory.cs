using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Setting;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Settings;
using StockManagementSystem.Web.Extensions;
using StockManagementSystem.Web.Kendoui.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Factories
{
    public class FormatSettingModelFactory : IFormatSettingModelFactory
    {
        private readonly IFormatSettingService _formatSettingService;

        public FormatSettingModelFactory(
            IFormatSettingService formatSettingService)
        {
            _formatSettingService = formatSettingService;
        }

        public async Task<FormatSettingContainerModel> PrepareFormatSettingContainerModel(
            FormatSettingContainerModel formatSettingContainerModel)
        {
            if (formatSettingContainerModel == null)
                throw new ArgumentNullException(nameof(formatSettingContainerModel));

            //prepare nested models
            await PrepareShelfFormatListModel(formatSettingContainerModel.ListShelfFormat);
            await PrepareBarcodeFormatListModel(formatSettingContainerModel.ListBarcodeFormat);

            return formatSettingContainerModel;
        }

        public async Task<ShelfSearchModel> PrepareShelfFormatSearchModel(ShelfSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);
        }

        public async Task<ShelfListModel> PrepareShelfFormatListModel(ShelfSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var shelfFormats = await _formatSettingService.GetAllShelfLocationFormatsAsync();

            if (shelfFormats == null)
                throw new ArgumentNullException(nameof(shelfFormats));

            var model = new ShelfListModel
            {
                Data = shelfFormats.PaginationByRequestModel(searchModel).Select(shelfFormat =>
                {
                    var shelfFormatsModel = shelfFormat.ToModel<ShelfModel>();
                    return shelfFormatsModel;
                }),
                Total = shelfFormats.Count
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

        public async Task<BarcodeSearchModel> PrepareBarcodeFormatSearchModel(BarcodeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
     
            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);
        }

        public async Task<BarcodeListModel> PrepareBarcodeFormatListModel(BarcodeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var barcodeFormats = await _formatSettingService.GetAllBarcodeFormatsAsync();

            var model = new BarcodeListModel
            {
                Data = barcodeFormats.Where(barcodeFormat => barcodeFormat.Format.Contains("Barcode"))
                    .Select(barcodeFormat =>
                    {
                        var barcodeFormatModel = barcodeFormat.ToModel<BarcodeModel>();
                       // barcodeFormatModel.Id = barcodeFormat.Id;

                        return barcodeFormatModel;
                    }),
                Total = barcodeFormats.Count
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
            if (searchModel.Filter?.Filters != null && searchModel.Filter.Filters.Any())
            {
                var filter = searchModel.Filter;
                model.Data = await model.Data.Filter(filter);
                model.Total = model.Data.Count();
            }

            return model;
        }
    }
}
