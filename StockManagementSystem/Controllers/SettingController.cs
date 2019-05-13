using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Media;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Factories;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Locations;
using StockManagementSystem.Models.OrderLimits;
using StockManagementSystem.Models.Replenishments;
using StockManagementSystem.Models.Setting;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Locations;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Media;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.OrderLimits;
using StockManagementSystem.Services.Replenishments;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Settings;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Controllers
{
    public class SettingController : BaseController
    {
        private readonly IOrderLimitService _orderLimitService;
        private readonly IReplenishmentService _replenishmentService;
        private readonly ILocationService _locationService;
        private readonly IFormatSettingService _formatSettingService;
        private readonly IOrderLimitModelFactory _orderLimitModelFactory;
        private readonly IReplenishmentModelFactory _replenishmentModelFactory;
        private readonly ILocationModelFactory _locationModelFactory;
        private readonly IFormatSettingModelFactory _formatSettingModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IWorkContext _workContext;
        private readonly ITenantContext _tenantContext;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IEncryptionService _encryptionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreService _storeService;
        private readonly ITenantService _tenantService;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly IRepository<ShelfLocation> _shelfLocationRepository;

        public SettingController(
            IOrderLimitService orderLimitService, 
            IReplenishmentService replenishmentService,
            ILocationService locationService, 
            IFormatSettingService formatSettingService,
            IOrderLimitModelFactory orderLimitModelFactory, 
            IReplenishmentModelFactory replenishmentModelFactory,
            ILocationModelFactory locationModelFactory, 
            IFormatSettingModelFactory formatSettingModelFactory,
            IPermissionService permissionService, 
            INotificationService notificationService, 
            IWorkContext workContext,
            ITenantContext tenantContext, 
            IConfiguration configuration, 
            IDateTimeHelper dateTimeHelper,
            IUserService userService, 
            IUserActivityService userActivityService, 
            IEncryptionService encryptionService,
            IGenericAttributeService genericAttributeService, 
            IStoreService storeService, 
            ITenantService tenantService,
            ISettingService settingService, 
            IPictureService pictureService, 
            ISettingModelFactory settingModelFactory,
            IRepository<ShelfLocation> shelfLocationRepository)
        {
            _orderLimitService = orderLimitService;
            _replenishmentService = replenishmentService;
            _locationService = locationService;
            _formatSettingService = formatSettingService;
            _orderLimitModelFactory = orderLimitModelFactory;
            _replenishmentModelFactory = replenishmentModelFactory;
            _locationModelFactory = locationModelFactory;
            _formatSettingModelFactory = formatSettingModelFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _workContext = workContext;
            _tenantContext = tenantContext;
            _configuration = configuration;
            _dateTimeHelper = dateTimeHelper;
            _userService = userService;
            _userActivityService = userActivityService;
            _encryptionService = encryptionService;
            _genericAttributeService = genericAttributeService;
            _storeService = storeService;
            _tenantService = tenantService;
            _settingService = settingService;
            _pictureService = pictureService;
            _settingModelFactory = settingModelFactory;
            _shelfLocationRepository = shelfLocationRepository;
        }

        #region Stock Order

        public async Task<IActionResult> Order()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var model = await _orderLimitModelFactory.PrepareOrderLimitSearchModel(new OrderLimitSearchModel());

            return View(model);
        }

        public async Task<IActionResult> GetStore()
        {
            var model = await _orderLimitModelFactory.PrepareOrderLimitSearchModel(new OrderLimitSearchModel());

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> AddOrderLimit(OrderLimitModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            OrderBranchMaster orderLimit = new OrderBranchMaster();

            if (ModelState.IsValid)
            {
                try
                {
                    var isExist = await _orderLimitService.IsStoreExistAsync(model.SelectedStoreIds);
                    if (isExist)
                    {
                        ModelState.AddModelError(string.Empty, "Store has existed in current order limit.");
                        _notificationService.ErrorNotification("Store has existed in current order limit.");
                        return new NullJsonResult();
                    }

                    orderLimit = new OrderBranchMaster
                    {
                        P_DeliveryPerWeek = model.DeliveryPerWeek,
                        P_Safety = model.Safety,
                        P_InventoryCycle = model.InventoryCycle,
                        P_OrderRatio = model.OrderRatio,
                        P_BranchNo = model.SelectedStoreIds,
                        Status = 1
                    };

                    await _orderLimitService.InsertOrderLimit(orderLimit);

                    return RedirectToAction("Order");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                    _notificationService.ErrorNotification(e.Message);

                    return Json(e.Message);
                }
            }
            else { return View(model); }
        }
        
        public async Task<IActionResult> AddOrderLimit()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var model = await _orderLimitModelFactory.PrepareOrderLimitModel(null, null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OrderLimitList(OrderLimitSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedKendoGridJson();

            var model = await _orderLimitModelFactory.PrepareOrderLimitListModel(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> EditOrder(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var orderLimit = await _orderLimitService.GetOrderLimitByIdAsync(id);
            if (orderLimit == null)
                return RedirectToAction("Order");

            var model = await _orderLimitModelFactory.PrepareOrderLimitModel(null, orderLimit);

            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> EditOrder(OrderLimitModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var orderLimit = await _orderLimitService.GetOrderLimitByIdAsync(model.Id);
            if (orderLimit == null)
                return RedirectToAction("Order");

            //validate stores
            var allStores = await _storeService.GetStores();
            var newStores = new List<Store>();
            foreach (var store in allStores)
                if (model.SelectedStoreIds == store.P_BranchNo)
                    newStores.Add(store);

            if (ModelState.IsValid)
                try
                {
                    orderLimit.P_DeliveryPerWeek = model.DeliveryPerWeek;
                    orderLimit.P_Safety = model.Safety;
                    orderLimit.P_InventoryCycle = model.InventoryCycle;
                    orderLimit.P_OrderRatio = model.OrderRatio;
                    orderLimit.P_BranchNo = model.SelectedStoreIds;

                    _orderLimitService.UpdateOrderLimit(orderLimit);

                    _notificationService.SuccessNotification("Order limit has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("Order");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("EditOrder", new {id = orderLimit.Id});
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }

            model = await _orderLimitModelFactory.PrepareOrderLimitModel(model, orderLimit);

            return View(model);
        }

        public async Task<IActionResult> DeleteOrderLimit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var orderLimit = await _orderLimitService.GetOrderLimitByIdAsync(id);
            if (orderLimit == null)
                return RedirectToAction("Order");

            try
            {
                _orderLimitService.DeleteOrderLimit(orderLimit);

                _notificationService.SuccessNotification("Order limit has been deleted successfully.");

                return RedirectToAction("Order");
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return RedirectToAction("EditOrder", new {id = orderLimit.Id});
            }
        }

        #endregion

        #region Approval

        [HttpGet]
        public IActionResult Approval()
        {
            return View("Approval");
        }

        #endregion

        //#region Location

        //public async Task<IActionResult> Location()
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
        //        return AccessDeniedView();

        //    var model = await _locationModelFactory.PrepareShelfLocationFormatSearchModel(new LocationSearchModel());
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> ListLocation(LocationSearchModel searchModel)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
        //        return AccessDeniedKendoGridJson();

        //    var model = await _locationModelFactory.PrepareShelfLocationFormatListModel(searchModel);
        //    return Json(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddLocation(LocationModel model)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
        //        return AccessDeniedView();

        //    if (!ModelState.IsValid)
        //        return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});

        //    try
        //    {
        //        ShelfLocationFormat shelfLocationFormat = new ShelfLocationFormat
        //        {
        //            Prefix = model.Prefix,
        //            Name = model.Name
        //        };

        //        shelfLocationFormat = model.ToEntity(shelfLocationFormat);

        //        await _locationService.InsertShelfLocationFormat(shelfLocationFormat);

        //        return new NullJsonResult();
        //    }
        //    catch (Exception e)
        //    {
        //        ModelState.AddModelError(string.Empty, e.Message);
        //        _notificationService.ErrorNotification(e.Message);

        //        return Json(e.Message);
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateLocation(LocationModel model)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
        //        return AccessDeniedView();

        //    if (!ModelState.IsValid)
        //        return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});

        //    var shelfLocationFormat = await _locationService.GetShelfLocationFormatByIdAsync(model.Id);
        //    shelfLocationFormat.Prefix = model.Prefix;
        //    shelfLocationFormat.Name = model.Name;

        //    shelfLocationFormat = model.ToEntity(shelfLocationFormat);

        //    _locationService.UpdateShelfLocationFormat(shelfLocationFormat);

        //    return new NullJsonResult();
        //}

        //public async Task<IActionResult> DeleteLocation(int id)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
        //        return AccessDeniedView();

        //    var shelfLocationFormat = await _locationService.GetShelfLocationFormatByIdAsync(id) ??
        //                              throw new ArgumentException("No prefix name found with the specified id",
        //                                  nameof(id));
        //    _locationService.DeleteShelfLocationFormat(shelfLocationFormat);

        //    _notificationService.SuccessNotification("Prefix name has been deleted successfully.");

        //    return new NullJsonResult();
        //}

        ////[HttpPost]
        ////public async Task<IActionResult> ExportTemplate(LocationSearchModel searchModel)
        ////{
        ////    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
        ////        return AccessDeniedView();

        ////    var model = await _locationModelFactory.PrepareShelfLocationFormatListModel(searchModel);

        ////    string sWebRootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        ////    string sFileName = @"TemplateFormat.xlsx";
        ////    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
        ////    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        ////    var memory = new MemoryStream();
        ////    using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
        ////    {
        ////        IWorkbook workbook;
        ////        workbook = new XSSFWorkbook();
        ////        ISheet excelSheet = workbook.CreateSheet("Template Format");
        ////        IRow row = excelSheet.CreateRow(0);

        ////        row.CreateCell(0).SetCellValue("Branch");
        ////        row.CreateCell(1).SetCellValue("Item");
        ////        row.CreateCell(2).SetCellValue("Prefix");
        ////        row.CreateCell(3).SetCellValue("Name");

        ////        row = excelSheet.CreateRow(1);
        ////        for (int i = 1; i <= model.Total; i++)
        ////        {
        ////            row.CreateCell(0).SetCellValue(" ");
        ////            row.CreateCell(1).SetCellValue(" ");
        ////            row.CreateCell(2).SetCellValue("");
        ////            row.CreateCell(3).SetCellValue("");
        ////        }

        ////        workbook.Write(fs);
        ////    }
        ////    using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
        ////    {
        ////        await stream.CopyToAsync(memory);
        ////    }
        ////    memory.Position = 0;
        ////    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        ////}


        ////    return new NullJsonResult();
        ////}

        //#endregion

        //#region Format Setting

        //public async Task<IActionResult> FormatSetting()
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    var model =
        //        await _formatSettingModelFactory.PrepareFormatSettingContainerModel(new FormatSettingContainerModel());
        //    return View(model);
        //}

        //#region Shelf Location

        //public async Task<IActionResult> ListShelf()
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    var model = await _formatSettingModelFactory.PrepareShelfFormatSearchModel(new ShelfSearchModel());

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> ListShelf(ShelfSearchModel searchModel)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedKendoGridJson();

        //    var model = await _formatSettingModelFactory.PrepareShelfFormatListModel(searchModel);
        //    return Json(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddShelf(ShelfModel model)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    if (string.IsNullOrEmpty(model.Prefix))
        //    {
        //        ModelState.AddModelError(string.Empty, "Prefix is required to add shelf location format.");
        //        _notificationService.ErrorNotification("Prefix is required to add shelf location format.");
        //        return new NullJsonResult();
        //    }
        //    if (string.IsNullOrEmpty(model.Name))
        //    {
        //        ModelState.AddModelError(string.Empty, "Name is required to add shelf location format.");
        //        _notificationService.ErrorNotification("Name is required to add shelf location format.");
        //        return new NullJsonResult();
        //    }

        //    try
        //    {
        //        var dataList = await _formatSettingService.GetAllShelfLocationFormatsAsync();

        //        if (dataList.Count >= 4)
        //        {
        //            _notificationService.WarningNotification("Add row limit to 4 only!");
        //        }
        //        else
        //        {
        //            var isExist = _formatSettingService.CheckFormatExist(model.Name, model.Prefix);

        //            if (!isExist)
        //            {
        //                FormatSetting shelfFormat = new FormatSetting
        //                {
        //                    Format = "Shelf",
        //                    Prefix = model.Prefix,
        //                    Name = model.Name
        //                };

        //                shelfFormat = model.ToEntity(shelfFormat);

        //                await _formatSettingService.InsertShelfLocationFormat(shelfFormat);
        //            }
        //            else
        //            {
        //                ModelState.AddModelError(string.Empty,
        //                    "Name and/or prefix already exist in Shelf Location format.");
        //                _notificationService.ErrorNotification(
        //                    "Name and/or prefix already exist in Shelf Location format.");
        //                return Json("Name and/or prefix already exist in Shelf Location format.");
        //            }
        //        }
        //        return new NullJsonResult();
        //    }
        //    catch (Exception e)
        //    {
        //        ModelState.AddModelError(string.Empty, e.Message);
        //        _notificationService.ErrorNotification(e.Message);

        //        return Json(e.Message);
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateShelf(ShelfModel model)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    if (!ModelState.IsValid)
        //        return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});

        //    var isExist = _formatSettingService.CheckFormatExist(model.Name, model.Prefix);

        //    if (!isExist)
        //    {
        //        var shelfFormat = await _formatSettingService.GetShelfLocationFormatByIdAsync(model.Id);
        //        shelfFormat.Prefix = model.Prefix;
        //        shelfFormat.Name = model.Name;

        //        shelfFormat = model.ToEntity(shelfFormat);

        //        _formatSettingService.UpdateShelfLocationFormat(shelfFormat);
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Name and/or prefix already exist in Shelf Location format.");
        //        _notificationService.ErrorNotification("Name and/or prefix already exist in Shelf Location format.");
        //        return Json("Name and/or prefix already exist in Shelf Location format.");
        //    }

        //    return new NullJsonResult();
        //}

        //public async Task<IActionResult> DeleteShelf(int id)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    var shelfFormat = await _formatSettingService.GetShelfLocationFormatByIdAsync(id) ??
        //                      throw new ArgumentException("No shelf location found", nameof(id));
        //    _formatSettingService.DeleteShelfLocationFormat(shelfFormat);

        //    return new NullJsonResult();
        //}

        //[HttpGet]
        //public async Task<IActionResult> DownloadTemplate(ShelfSearchModel searchModel)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    var model = await _formatSettingModelFactory.PrepareShelfFormatListModel(searchModel);
        //    if (model == null)
        //        return new NullJsonResult();

        //    try
        //    {
        //        using (ExcelPackage excelPackage = new ExcelPackage())
        //        {
        //            //Set some properties of the Excel document
        //            excelPackage.Workbook.Properties.Author = "Administrator";
        //            excelPackage.Workbook.Properties.Title = "Format Template";
        //            excelPackage.Workbook.Properties.Subject = "Download Shelf Location";
        //            excelPackage.Workbook.Properties.Created = DateTime.Now;

        //            //Create the WorkSheet
        //            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

        //            //Header row style cells C1 - C3
        //            worksheet.Cells["A1:C3"].Style.Font.Size = 12;
        //            worksheet.Cells["A1:C3"].Style.Font.Bold = true;
        //            worksheet.Cells["A1:C3"].Style.Border.Top.Style = ExcelBorderStyle.Hair;

        //            //Set [line, column]
        //            worksheet.Cells[1, 1].Value = "Branch No";
        //            worksheet.Cells[1, 2].Value = "Stock Code";
        //            worksheet.Cells[1, 3].Value = "Location";

        //            //Save excel file
        //            string rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //            string fileName = @"TemplateFormat.xlsx";
        //            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
        //            excelPackage.SaveAs(file);

        //            if (model.Total > 0)
        //                _notificationService.SuccessNotification("Successful download template.");
        //            else
        //                _notificationService.ErrorNotification("Failed download template.");
        //        }
        //        return RedirectToAction("FormatSetting");
        //    }
        //    catch (Exception e)
        //    {
        //        _notificationService.ErrorNotification(e.Message);
        //        return Json(e.Message);
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> UploadTemplate()
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    try
        //    {
        //        var prefixFormat = await _formatSettingService.GetAllShelfLocationFormatsAsync();
        //        List<ShelfLocation> data = new List<ShelfLocation>();
        //        string locationFormat;
        //        int x = 0;

        //        //read the Excel file
        //        string rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //        string fileName = @"TemplateFormat.xlsx";
        //        FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
        //        //create a new Excel package
        //        using (ExcelPackage excelPackage = new ExcelPackage(file))
        //        {
        //            //loop all worksheets
        //            foreach (ExcelWorksheet worksheet in excelPackage.Workbook.Worksheets)
        //            {
        //                //loop rows start at 2nd row
        //                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
        //                {
        //                    x = 0;
        //                    locationFormat = worksheet.Cells[i, 3].Value.ToString();
        //                    string[] listPrefix = Regex.Split(locationFormat, @"\d[0-9]{1,3}|\W");
        //                    //bool formatMatch = Regex.IsMatch(locationFormat, @"\w[0-9]{2,4}"); *Set prefix format match with 2-4 digit

        //                    foreach (var item in prefixFormat.ToArray())
        //                    {
        //                        if (listPrefix[x].Equals(item.Prefix))
        //                        {
        //                            x++;
        //                            //formatMatch = true;
        //                        }
        //                        else
        //                        {
        //                            //formatMatch = false;
        //                            _notificationService.ErrorNotification("Prefix are invalid! Location format digit limit to 4.");
        //                            return RedirectToAction("FormatSetting");
        //                        }
        //                    }

        //                    data.Add(new ShelfLocation
        //                    {
        //                        StoreId = Int32.Parse(worksheet.Cells[i, 1].Value.ToString()),
        //                        Stock_Code = worksheet.Cells[i, 2].Value.ToString(),
        //                        Location = worksheet.Cells[i, 3].Value.ToString(),
        //                        CreatedOnUtc = DateTime.Now
        //                    });

        //                    //if (!formatMatch) 
        //                    //{
        //                    //    _notificationService.ErrorNotification("Location format are invalid!");
        //                    //    return RedirectToAction("FormatSetting");
        //                    //}
        //                    //else
        //                    //{
        //                    //    data.Add(new ShelfLocation
        //                    //    {
        //                    //        StoreId = Int32.Parse(worksheet.Cells[i, 1].Value.ToString()),
        //                    //        Stock_Code = worksheet.Cells[i, 2].Value.ToString(),
        //                    //        Location = worksheet.Cells[i, 3].Value.ToString(),
        //                    //        CreatedBy = "Administrator",
        //                    //        CreatedOnUtc = DateTime.Now
        //                    //    });
        //                    //}
        //                }
        //                _shelfLocationRepository.Update(data);
        //            }

        //            if (data.Count > 0)
        //                _notificationService.SuccessNotification("Successful upload template.");
        //            else
        //                _notificationService.ErrorNotification("Failed upload template.");
        //        }
        //        return RedirectToAction("FormatSetting");
        //    }
        //    catch (Exception e)
        //    {
        //        _notificationService.ErrorNotification(e.Message);
        //        return Json(e.Message);
        //    }
        //}       

        //#endregion

        //#region RTE Barcode

        //public async Task<IActionResult> ListBarcode()
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    var model = await _formatSettingModelFactory.PrepareBarcodeFormatSearchModel(new BarcodeSearchModel());

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> ListBarcode(BarcodeSearchModel searchModel)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedKendoGridJson();

        //    var model = await _formatSettingModelFactory.PrepareBarcodeFormatListModel(searchModel);

        //    return Json(model);
        //}

        //[HttpGet]
        //public async Task<IActionResult> SortBarcode(string data)
        //{
        //    data = data.Replace('"', ' ');
        //    var r = new Regex(@"Name : (.+?) , Length");
        //    var mc = r.Matches(data);
        //    var arr = new List<string>();

        //    foreach (Match match in mc)
        //    foreach (Capture capture in match.Captures)
        //        arr.Add(capture.Value.Split(new[] {"Name : "}, StringSplitOptions.None)[1].Split(',')[0]
        //            .Trim());

        //    var barcodeFormat = await _formatSettingService.GetAllBarcodeFormatsAsync();
        //    foreach (var item in barcodeFormat)
        //        for (var i = 0; i < arr.Count(); i++)
        //            if (item.Name == arr[i])
        //            {
        //                var seq = i + 1;
        //                item.Sequence = seq;

        //                _formatSettingService.UpdateBarcodeFormat(item);
        //            }

        //    return new NullJsonResult();
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateBarcode(BarcodeModel model)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    if (!ModelState.IsValid)
        //        return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});

        //    var isExist = _formatSettingService.CheckFormatExist(model.Name, null);

        //    if (!isExist)
        //    {
        //        var barcodeFormat = await _formatSettingService.GetBarcodeFormatByIdAsync(model.Id);
        //        barcodeFormat.Id = model.Id;
        //        barcodeFormat.Name = model.Name;

        //        _formatSettingService.UpdateBarcodeFormat(barcodeFormat);
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Name already exist in RTE barcode format.");
        //        _notificationService.ErrorNotification("Name already exist in RTE barcode format.");
        //        return Json("Name already exist in RTE barcode format.");
        //    }

        //    return new NullJsonResult();
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddBarcode(BarcodeModel model)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    if (string.IsNullOrEmpty(model.Name))
        //    {
        //        ModelState.AddModelError(string.Empty, "Name is required to add RTE barcode format.");
        //        _notificationService.ErrorNotification("Name is required to add RTE barcode format.");
        //    }
        //    try
        //    {
        //        var dataList = await _formatSettingService.GetAllBarcodeFormatsAsync();

        //        if (dataList.Count >= 4)
        //        {
        //            _notificationService.WarningNotification("Add row limit to 4 only!");
        //        }
        //        else
        //        {
        //            var isExist = _formatSettingService.CheckFormatExist(model.Name, null);

        //            if (!isExist)
        //            {
        //                var counter = dataList.Count;
        //                FormatSetting barcodeFormat = new FormatSetting
        //                {
        //                    Format = "Barcode",
        //                    Length = Convert.ToInt32(_configuration["RTEBarcodeLength"]),
        //                    Name = model.Name,
        //                    Sequence = counter + 1
        //                };

        //                await _formatSettingService
        //                    .InsertShelfLocationFormat(barcodeFormat); //Uses same function as shelf location
        //            }
        //            else
        //            {
        //                ModelState.AddModelError(string.Empty, "Name already exist in RTE barcode format.");
        //                _notificationService.ErrorNotification("Name already exist in RTE barcode format.");
        //                return Json("Name already exist in RTE barcode format.");
        //            }
        //        }

        //        return new NullJsonResult();
        //    }
        //    catch (Exception e)
        //    {
        //        ModelState.AddModelError(string.Empty, e.Message);
        //        _notificationService.ErrorNotification(e.Message);

        //        return Json(e.Message);
        //    }
        //}

        //public async Task<IActionResult> DeleteBarcode(int id)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
        //        return AccessDeniedView();

        //    var barcodeFormat = await _formatSettingService.GetBarcodeFormatByIdAsync(id);
        //    if (barcodeFormat == null)
        //        return new NullJsonResult();

        //    try
        //    {
        //        _formatSettingService.DeleteShelfLocationFormat(barcodeFormat); //Uses same function as shelf location

        //        var nextSeq = barcodeFormat.Sequence + 1;
        //        if (nextSeq > 0 && nextSeq <= 4)
        //            for (var i = nextSeq; i <= 4; i++)
        //            {
        //                var counter = i - 1;
        //                var nextBarcodeFormat = await _formatSettingService.GetBarcodeFormatBySeqAsync(i);
        //                if (nextBarcodeFormat != null)
        //                {
        //                    nextBarcodeFormat.Sequence = counter;
        //                    _formatSettingService.UpdateBarcodeFormat(nextBarcodeFormat);
        //                }
        //            }

        //        return new NullJsonResult();
        //    }
        //    catch (Exception e)
        //    {
        //        _notificationService.ErrorNotification(e.Message);
        //        return new NullJsonResult();
        //    }
        //}

        //#endregion

        //#endregion

        //#region Replenishment

        //public async Task<IActionResult> Replenishment()
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReplenishmentSetting))
        //        return AccessDeniedView();

        //    var model =
        //        await _replenishmentModelFactory.PrepareReplenishmentSearchModel(new ReplenishmentSearchModel());

        //    return View(model);
        //}

        //public async Task<IActionResult> GetStoreReplenishment()
        //{
        //    var model =
        //        await _replenishmentModelFactory.PrepareReplenishmentSearchModel(new ReplenishmentSearchModel());

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddReplenishment(ReplenishmentModel model)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
        //        return AccessDeniedView();

        //    if (model.SelectedStoreIds.Count == 0)
        //    {
        //        ModelState.AddModelError(string.Empty, "Store is required to configure replenishment");
        //        _notificationService.ErrorNotification("Store is required to configure replenishment");
        //        return new NullJsonResult();
        //    }

        //    try
        //    {
        //        var replenishment = new Replenishment
        //        {
        //            BufferDays = model.BufferDays,
        //            ReplenishmentQty = model.ReplenishmentQty,
        //            ReplenishmentStores = new List<ReplenishmentStore>()
        //        };

        //        //Add store
        //        foreach (var store in model.SelectedStoreIds)
        //        {
        //            var replenishmentStore = new ReplenishmentStore
        //            {
        //                ReplenishmentId = replenishment.Id,
        //                StoreId = store
        //            };

        //            replenishment.ReplenishmentStores.Add(replenishmentStore);
        //        }

        //        await _replenishmentService.InsertReplenishment(replenishment);

        //        return new NullJsonResult();
        //    }
        //    catch (Exception e)
        //    {
        //        ModelState.AddModelError(string.Empty, e.Message);
        //        _notificationService.ErrorNotification(e.Message);

        //        return Json(e.Message);
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> ReplenishmentList(ReplenishmentSearchModel searchModel)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReplenishmentSetting))
        //        return AccessDeniedKendoGridJson();

        //    var model = await _replenishmentModelFactory.PrepareReplenishmentListModel(searchModel);

        //    return Json(model);
        //}

        //public async Task<IActionResult> EditReplenishment(int id)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReplenishmentSetting))
        //        return AccessDeniedView();

        //    var replenishment = await _replenishmentService.GetReplenishmentByIdAsync(id);
        //    if (replenishment == null)
        //        return RedirectToAction("Replenishment");

        //    var model = await _replenishmentModelFactory.PrepareReplenishmentModel(null, replenishment);

        //    return View(model);
        //}

        //[HttpPost]
        //[ParameterBasedOnFormName("save-continue", "continueEditing")]
        //[FormValueRequired("save", "save-continue")]
        //public async Task<IActionResult> EditReplenishment(ReplenishmentModel model, bool continueEditing)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReplenishmentSetting))
        //        return AccessDeniedView();

        //    var replenishment = await _replenishmentService.GetReplenishmentByIdAsync(model.Id);
        //    if (replenishment == null)
        //        return RedirectToAction("Replenishment");

        //    var allStores = await _storeService.GetStores();
        //    var newStores = new List<Store>();
        //    foreach (var store in allStores)
        //        if (model.SelectedStoreIds.Contains(store.P_BranchNo))
        //            newStores.Add(store);

        //    if (model.SelectedStoreIds.Count == 0)
        //    {
        //        _notificationService.ErrorNotification("Store is required");
        //        model = await _replenishmentModelFactory.PrepareReplenishmentModel(model, replenishment);
        //        model.SelectedStoreIds = new List<int>();

        //        return View(model);
        //    }

        //    if (ModelState.IsValid)
        //        try
        //        {
        //            replenishment.BufferDays = model.BufferDays;
        //            replenishment.ReplenishmentQty = model.ReplenishmentQty;

        //            foreach (var store in allStores)
        //                if (model.SelectedStoreIds.Contains(store.P_BranchNo))
        //                {
        //                    //new store
        //                    if (replenishment.ReplenishmentStores.Count(mapping =>
        //                            mapping.StoreId == store.P_BranchNo) == 0)
        //                    {
        //                        var replenishmentStore = new ReplenishmentStore
        //                        {
        //                            ReplenishmentId = replenishment.Id,
        //                            StoreId = store.P_BranchNo
        //                        };


        //                        replenishment.ReplenishmentStores.Add(replenishmentStore);
        //                    }
        //                }
        //                else
        //                {
        //                    //remove store
        //                    if (replenishment.ReplenishmentStores.Count(mapping =>
        //                            mapping.StoreId == store.P_BranchNo) > 0)
        //                        _replenishmentService.DeleteReplenishmentStore(model.Id, store);
        //                }

        //            _replenishmentService.UpdateReplenishment(replenishment);

        //            _notificationService.SuccessNotification("Replenishment has been updated successfully.");

        //            if (!continueEditing)
        //                return RedirectToAction("Replenishment");

        //            //selected tab
        //            SaveSelectedTabName();

        //            return RedirectToAction("EditReplenishment", new {id = replenishment.Id});
        //        }
        //        catch (Exception e)
        //        {
        //            _notificationService.ErrorNotification(e.Message);
        //        }

        //    model = await _replenishmentModelFactory.PrepareReplenishmentModel(model, replenishment);

        //    return View(model);
        //}

        //public async Task<IActionResult> DeleteReplenishment(int id)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReplenishmentSetting))
        //        return AccessDeniedView();

        //    var replenishment = await _replenishmentService.GetReplenishmentByIdAsync(id);
        //    if (replenishment == null)
        //        return RedirectToAction("Replenishment");

        //    try
        //    {
        //        _replenishmentService.DeleteReplenishment(replenishment);

        //        _notificationService.SuccessNotification("Replenishment has been deleted successfully.");

        //        return RedirectToAction("Replenishment");
        //    }
        //    catch (Exception e)
        //    {
        //        _notificationService.ErrorNotification(e.Message);
        //        return RedirectToAction("EditReplenishment", new {id = replenishment.Id});
        //    }
        //}

        //#endregion

        #region Tenant scope configuration

        public async Task<IActionResult> ChangeTenantScopeConfiguration(int tenantId, string returnUrl = "")
        {
            var tenant = _tenantService.GetTenantById(tenantId);
            if (tenant != null || tenantId == 0)
                await _genericAttributeService.SaveAttributeAsync(_workContext.CurrentUser,
                    UserDefaults.TenantScopeConfigurationAttribute, tenantId);

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home");

            return Redirect(returnUrl);
        }

        #endregion

        #region Settings

        public async Task<IActionResult> GeneralCommon()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = await _settingModelFactory.PrepareGeneralCommonSettingsModel();

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> GeneralCommon(GeneralCommonSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var tenantScope = _tenantContext.ActiveTenantScopeConfiguration;

            //common settings
            var commonSettings = _settingService.LoadSetting<CommonSettings>(tenantScope);
            commonSettings.LogoPictureId = model.CommonSettings.LogoPictureId;
            commonSettings.UseResponseCompression = model.CommonSettings.UseResponseCompression;

            await _settingService.SaveSettingOverridablePerTenant(commonSettings, x => x.LogoPictureId, model.CommonSettings.LogoPictureId_OverrideForTenant, tenantScope, false);
            await _settingService.SaveSettingOverridablePerTenant(commonSettings, x => x.UseResponseCompression, model.CommonSettings.UseResponseCompression_OverrideForTenant, tenantScope, false);

            _settingService.ClearCache();

            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(tenantScope);
            if (securitySettings.AllowedIpAddresses == null)
                securitySettings.AllowedIpAddresses = new List<string>();
            securitySettings.AllowedIpAddresses.Clear();

            if (!string.IsNullOrEmpty(model.SecuritySettings.AllowedIpAddresses))
                foreach (var s in model.SecuritySettings.AllowedIpAddresses.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    if (!string.IsNullOrWhiteSpace(s))
                        securitySettings.AllowedIpAddresses.Add(s.Trim());

            securitySettings.ForceSslForAllPages = model.SecuritySettings.ForceSslForAllPages;
            securitySettings.EnableXsrfProtection = model.SecuritySettings.EnableXsrfProtection;

            _settingService.SaveSetting(securitySettings);

            await _userActivityService.InsertActivityAsync("EditSettings", "Edited settings");

            _notificationService.SuccessNotification("The settings have been updated successfully.");

            return RedirectToAction("GeneralCommon");
        }

        public async Task<IActionResult> UserAdmin()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = await _settingModelFactory.PrepareUserAdminSettingsModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserAdmin(UserAdminSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var tenantScope = _tenantContext.ActiveTenantScopeConfiguration;
            var userSettings = _settingService.LoadSetting<UserSettings>(tenantScope);

            var lastUsernameValidationRule = userSettings.UsernameValidationRule;
            var lastUsernameValidationEnabledValue = userSettings.UsernameValidationEnabled;
            var lastUsernameValidationUseRegexValue = userSettings.UsernameValidationUseRegex;

            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(tenantScope);

            userSettings = model.UserSettings.ToSettings(userSettings);

            if (userSettings.UsernameValidationEnabled && userSettings.UsernameValidationUseRegex)
            {
                try
                {
                    //validate regex rule
                    var unused = Regex.IsMatch("test_user_name", userSettings.UsernameValidationRule);
                }
                catch (ArgumentException)
                {
                    //restoring previous settings
                    userSettings.UsernameValidationRule = lastUsernameValidationRule;
                    userSettings.UsernameValidationEnabled = lastUsernameValidationEnabledValue;
                    userSettings.UsernameValidationUseRegex = lastUsernameValidationUseRegexValue;

                    _notificationService.ErrorNotification("The regular expression for username validation is incorrect");
                }
            }

            _settingService.SaveSetting(userSettings);

            dateTimeSettings.DefaultTimeZoneId = model.DateTimeSettings.DefaultTimeZoneId;
            dateTimeSettings.AllowUsersToSetTimeZone = model.DateTimeSettings.AllowUsersToSetTimeZone;
            _settingService.SaveSetting(dateTimeSettings);

            await _userActivityService.InsertActivityAsync("EditSettings", "Edited settings");

            _notificationService.SuccessNotification("The settings have been updated successfully.");

            SaveSelectedTabName();

            return RedirectToAction("UserAdmin");
        }

        public async Task<IActionResult> Media()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = await _settingModelFactory.PrepareMediaSettingsModel();

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> Media(MediaSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var tenantScope = _tenantContext.ActiveTenantScopeConfiguration;
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(tenantScope);
            mediaSettings = model.ToSettings(mediaSettings);

            await _settingService.SaveSettingOverridablePerTenant(mediaSettings, x => x.AvatarPictureSize, model.AvatarPictureSize_OverrideForTenant, tenantScope, false);
            await _settingService.SaveSettingOverridablePerTenant(mediaSettings, x => x.MaximumImageSize, model.MaximumImageSize_OverrideForTenant, tenantScope, false);
            await _settingService.SaveSettingOverridablePerTenant(mediaSettings, x => x.DefaultImageQuality, model.DefaultImageQuality_OverrideForTenant, tenantScope, false);

            _settingService.ClearCache();

            await _userActivityService.InsertActivityAsync("EditSettings", "Edited settings");

            _notificationService.SuccessNotification("The settings have been updated successfully.");

            return RedirectToAction("Media");
        }

        [HttpPost, ActionName("Media")]
        [FormValueRequired("change-picture-storage")]
        public async Task<IActionResult> ChangePictureStorage()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _pictureService.StoreInDb = !_pictureService.StoreInDb;

            await _userActivityService.InsertActivityAsync("EditSettings", "Edited settings");

            _notificationService.SuccessNotification("The settings have been updated successfully.");

            return RedirectToAction("Media");
        }

        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public async Task<IActionResult> ChangeEncryptionKey(GeneralCommonSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var tenantScope = _tenantContext.ActiveTenantScopeConfiguration;
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(tenantScope);

            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = string.Empty;

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (string.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new DefaultException("Encryption private key must be 16 characters long.");

                var oldEncryptionPrivateKey = securitySettings.EncryptionKey;
                if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                    throw new DefaultException("The new encryption key is the same as the old one.");

                //update password information
                var userPasswords = _userService.GetUserPasswords(passwordFormat: PasswordFormat.Encrypted);
                foreach (var userPassword in userPasswords)
                {
                    var decryptedPassword = _encryptionService.DecryptText(userPassword.Password, oldEncryptionPrivateKey);
                    var encryptedPassword = _encryptionService.EncryptText(decryptedPassword, newEncryptionPrivateKey);

                    userPassword.Password = encryptedPassword;
                    _userService.UpdateUserPassword(userPassword);
                }

                securitySettings.EncryptionKey = newEncryptionPrivateKey;
                _settingService.SaveSetting(securitySettings);

                _notificationService.SuccessNotification("Encryption key changed");
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification(ex);
            }

            return RedirectToAction("GeneralCommon");
        }

        #endregion
    }
}