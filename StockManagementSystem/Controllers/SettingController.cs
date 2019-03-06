using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Factories;
using StockManagementSystem.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Models.Locations;
using StockManagementSystem.Models.OrderLimits;
using StockManagementSystem.Services.Locations;
using StockManagementSystem.Services.Messages;
using StockManagementSystem.Services.OrderLimits;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Kendoui;
using StockManagementSystem.Web.Kendoui.Extensions;
using StockManagementSystem.Web.Mvc;
using StockManagementSystem.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Services.Settings;
using StockManagementSystem.Models.Setting;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using StockManagementSystem.Models.Replenishments;
using StockManagementSystem.Services.Replenishments;

namespace StockManagementSystem.Controllers
{
    public class SettingController : BaseController
    {
        private readonly IOrderLimitService _orderLimitService;
        private readonly IReplenishmentService _replenishmentService;
        private readonly IStoreService _storeService;
        private readonly ILocationService _locationService;
        private readonly IFormatSettingService _formatSettingService;
        private readonly IRepository<Approval> _approvalRepository;
        private readonly IRepository<OrderLimit> _orderLimitRepository;
        private readonly IRepository<OrderLimitStore> _orderLimitStoreRepository;
        private readonly IRepository<Replenishment> _replenishmentRepository;
        private readonly IRepository<ReplenishmentStore> _replenishmentStoreRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<ShelfLocation> _shelfLocationRepository;
        private readonly IRepository<ShelfLocationFormat> _shelfLocationFormatRepository;
        private readonly IRepository<FormatSetting> _formatSettingRepository;
        private readonly IOrderLimitModelFactory _orderLimitModelFactory;
        private readonly IReplenishmentModelFactory _replenishmentModelFactory;
        private readonly ILocationModelFactory _locationModelFactory;
        private readonly IFormatSettingModelFactory _formatSettingModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _iconfiguration;
        private readonly ILogger _logger;

        #region Constructor

        public SettingController(
            IOrderLimitService orderLimitService,
            IReplenishmentService replenishmentService,
            IStoreService storeService,
            ILocationService locationService,
            IFormatSettingService formatSettingService,
            IRepository<Approval> approvalRepository,
            IRepository<OrderLimit> orderLimitRepository,
            IRepository<OrderLimitStore> orderLimitStoreRepository,
            IRepository<Replenishment> replenishmentRepository,
            IRepository<ReplenishmentStore> replenishmentStoreRepository,
            IRepository<Store> storeRepository,
            IRepository<Item> itemRepository,
            IRepository<ShelfLocation> shelfLocationRepository,
            IRepository<ShelfLocationFormat> shelfLocationFormatRepository,
            IRepository<FormatSetting> formatSettingRepository,
            IOrderLimitModelFactory orderLimitModelFactory,
            IReplenishmentModelFactory replenishmentModelFactory,
            ILocationModelFactory locationModelFactory,
            IFormatSettingModelFactory _formatSettingModelFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            IConfiguration iconfiguration,
            ILoggerFactory loggerFactory)
        {
            this._orderLimitService = orderLimitService;
            this._replenishmentService = replenishmentService;
            this._storeService = storeService;
            this._locationService = locationService;
            this._formatSettingService = formatSettingService;
            this._approvalRepository = approvalRepository;
            this._orderLimitRepository = orderLimitRepository;
            this._orderLimitStoreRepository = orderLimitStoreRepository;
            this._replenishmentRepository = replenishmentRepository;
            this._replenishmentStoreRepository = replenishmentStoreRepository;
            this._storeRepository = storeRepository;
            this._itemRepository = itemRepository;
            this._shelfLocationRepository = shelfLocationRepository;
            this._shelfLocationFormatRepository = shelfLocationFormatRepository;
            this._formatSettingRepository = formatSettingRepository;
            this._orderLimitModelFactory = orderLimitModelFactory;
            this._replenishmentModelFactory = replenishmentModelFactory;
            this._locationModelFactory = locationModelFactory;
            this._formatSettingModelFactory = _formatSettingModelFactory;
            _iconfiguration = iconfiguration;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _logger = loggerFactory.CreateLogger<SettingController>();
        }

        public ILogger Logger { get; }

        #endregion

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
        public async Task<IActionResult> AddOrderLimit(OrderLimitModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            if (model.SelectedStoreIds.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Store is required to register a device");
                _notificationService.ErrorNotification("Store is required to register a device");
                return new NullJsonResult();
            }

            try
            {
                OrderLimit orderLimit = new OrderLimit
                {
                    //Percentage = model.Percentage, //Remove Percentage criteria; Not required - 05032019
                    DaysofSales = model.DaysofSales,
                    DaysofStock = model.DaysofStock,
                    OrderLimitStores = new List<OrderLimitStore>()
                };

                //Add store
                foreach (var store in model.SelectedStoreIds)
                {
                    OrderLimitStore orderLimitStore = new OrderLimitStore
                    {
                        OrderLimitId = orderLimit.Id,
                        StoreId = store
                    };

                    orderLimit.OrderLimitStores.Add(orderLimitStore);
                }

                await _orderLimitService.InsertOrderLimit(orderLimit);

                return new NullJsonResult();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                _notificationService.ErrorNotification(e.Message);

                return Json(e.Message);
            }
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

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> EditOrder(OrderLimitModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            var orderLimit = await _orderLimitService.GetOrderLimitByIdAsync(model.Id);
            if (orderLimit == null)
                return RedirectToAction("Order");

            //validate stores
            var allStores = await _storeService.GetStoresAsync();
            var newStores = new List<Store>();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                    newStores.Add(store);
            }

            if (model.SelectedStoreIds.Count == 0)
            {
                _notificationService.ErrorNotification("Store is required");
                model = await _orderLimitModelFactory.PrepareOrderLimitModel(model, orderLimit);
                model.SelectedStoreIds = new List<int>();

                return View(model);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //orderLimit.Percentage = model.Percentage; //Remove Percentage criteria; Not required - 05032019
                    orderLimit.DaysofSales = model.DaysofSales;
                    orderLimit.DaysofStock = model.DaysofStock;

                    //stores

                    List<OrderLimitStore> orderLimitStoreList = new List<OrderLimitStore>();

                    foreach (var store in allStores)
                    {
                        if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                        {
                            //new store
                            if (orderLimit.OrderLimitStores.Count(mapping => mapping.StoreId == store.P_BranchNo) == 0)
                            {
                                OrderLimitStore orderLimitStore = new OrderLimitStore
                                {
                                    OrderLimitId = orderLimit.Id,
                                    StoreId = store.P_BranchNo
                                };


                                orderLimit.OrderLimitStores.Add(orderLimitStore);
                            }
                        }
                        else
                        {
                            //remove store
                            if (orderLimit.OrderLimitStores.Count(mapping => mapping.StoreId == store.P_BranchNo) > 0)
                                _orderLimitService.DeleteOrderLimitStore(model.Id, store);
                        }
                    }

                    _orderLimitService.UpdateOrderLimit(orderLimit);

                    _notificationService.SuccessNotification("Order limit has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("Order");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("EditOrder", new { id = orderLimit.Id });
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }
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
                return RedirectToAction("EditOrder", new { id = orderLimit.Id });
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

        #region Location

        public async Task<IActionResult> Location()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
                return AccessDeniedView();

            var model = await _locationModelFactory.PrepareShelfLocationFormatSearchModel(new LocationSearchModel());
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ListLocation(LocationSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
                return AccessDeniedKendoGridJson();

            var model = await _locationModelFactory.PrepareShelfLocationFormatListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation(LocationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            try
            {
                ShelfLocationFormat shelfLocationFormat = new ShelfLocationFormat();
                shelfLocationFormat.Prefix = model.Prefix;
                shelfLocationFormat.Name = model.Name;

                shelfLocationFormat = model.ToEntity(shelfLocationFormat);

                await _locationService.InsertShelfLocationFormat(shelfLocationFormat);

                return new NullJsonResult();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                _notificationService.ErrorNotification(e.Message);

                return Json(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLocation(LocationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            var shelfLocationFormat = await _locationService.GetShelfLocationFormatByIdAsync(model.Id);
            shelfLocationFormat.Prefix = model.Prefix;
            shelfLocationFormat.Name = model.Name;

            shelfLocationFormat = model.ToEntity(shelfLocationFormat);

            _locationService.UpdateShelfLocationFormat(shelfLocationFormat);

            return new NullJsonResult();
        }

        public async Task<IActionResult> DeleteLocation(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
                return AccessDeniedView();

            var shelfLocationFormat = await _locationService.GetShelfLocationFormatByIdAsync(id) ?? throw new ArgumentException("No prefix name found with the specified id", nameof(id));
            _locationService.DeleteShelfLocationFormat(shelfLocationFormat);

            _notificationService.SuccessNotification("Prefix name has been deleted successfully.");

            return new NullJsonResult();
        }

        //[HttpPost]
        //public async Task<IActionResult> ExportTemplate(LocationSearchModel searchModel)
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageLocation))
        //        return AccessDeniedView();

        //    var model = await _locationModelFactory.PrepareShelfLocationFormatListModel(searchModel);

        //    string sWebRootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //    string sFileName = @"TemplateFormat.xlsx";
        //    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
        //    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    var memory = new MemoryStream();
        //    using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
        //    {
        //        IWorkbook workbook;
        //        workbook = new XSSFWorkbook();
        //        ISheet excelSheet = workbook.CreateSheet("Template Format");
        //        IRow row = excelSheet.CreateRow(0);

        //        row.CreateCell(0).SetCellValue("Branch");
        //        row.CreateCell(1).SetCellValue("Item");
        //        row.CreateCell(2).SetCellValue("Prefix");
        //        row.CreateCell(3).SetCellValue("Name");

        //        row = excelSheet.CreateRow(1);
        //        for (int i = 1; i <= model.Total; i++)
        //        {
        //            row.CreateCell(0).SetCellValue(" ");
        //            row.CreateCell(1).SetCellValue(" ");
        //            row.CreateCell(2).SetCellValue("");
        //            row.CreateCell(3).SetCellValue("");
        //        }

        //        workbook.Write(fs);
        //    }
        //    using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;
        //    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        //}


        //    return new NullJsonResult();
        //}

        #endregion

        #region Format Setting

        public async Task<IActionResult> FormatSetting()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedView();

            var model = await _formatSettingModelFactory.PrepareFormatSettingContainerModel(new FormatSettingContainerModel());
            return View(model);
        }

        #region Shelf Location

        public async Task<IActionResult> ListShelf()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedView();

            var model = await _formatSettingModelFactory.PrepareShelfFormatSearchModel(new ShelfSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ListShelf(ShelfSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedKendoGridJson();

            var model = await _formatSettingModelFactory.PrepareShelfFormatListModel(searchModel);
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddShelf(ShelfModel model)
        {
             if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(model.Prefix))
            {
                ModelState.AddModelError(string.Empty, "Prefix is required to add shelf location format.");
                _notificationService.ErrorNotification("Prefix is required to add shelf location format.");
                return new NullJsonResult();
            }
            else if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError(string.Empty, "Name is required to add shelf location format.");
                _notificationService.ErrorNotification("Name is required to add shelf location format.");
                return new NullJsonResult();
            }

            try
            {
                var dataList = await _formatSettingService.GetAllShelfLocationFormatsAsync();

                if (dataList.Count >= 4)
                {
                    _notificationService.WarningNotification("Add row limit to 4 only!");
                }
                else
                {
                    bool isExist = _formatSettingService.CheckFormatExist(model.Name, model.Prefix);

                    if (!isExist)
                    {
                        FormatSetting shelfFormat = new FormatSetting();
                        shelfFormat.Format = "Shelf";
                        shelfFormat.Prefix = model.Prefix;
                        shelfFormat.Name = model.Name;

                        shelfFormat = model.ToEntity(shelfFormat);

                        await _formatSettingService.InsertShelfLocationFormat(shelfFormat);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Name and/or prefix already exist in Shelf Location format.");
                        _notificationService.ErrorNotification("Name and/or prefix already exist in Shelf Location format.");
                        return Json("Name and/or prefix already exist in Shelf Location format.");
                    }
                }
                return new NullJsonResult();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                _notificationService.ErrorNotification(e.Message);

                return Json(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateShelf(ShelfModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            bool isExist = _formatSettingService.CheckFormatExist(model.Name, model.Prefix);

            if (!isExist)
            {
                var shelfFormat = await _formatSettingService.GetShelfLocationFormatByIdAsync(model.Id);
                shelfFormat.Prefix = model.Prefix;
                shelfFormat.Name = model.Name;

                shelfFormat = model.ToEntity(shelfFormat);

                _formatSettingService.UpdateShelfLocationFormat(shelfFormat);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Name and/or prefix already exist in Shelf Location format.");
                _notificationService.ErrorNotification("Name and/or prefix already exist in Shelf Location format.");
                return Json("Name and/or prefix already exist in Shelf Location format.");
            }

            return new NullJsonResult();
        }

        public async Task<IActionResult> DeleteShelf(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedView();

            var shelfFormat = await _formatSettingService.GetShelfLocationFormatByIdAsync(id) ?? throw new ArgumentException("No shelf location found", nameof(id));
            _formatSettingService.DeleteShelfLocationFormat(shelfFormat);

            return new NullJsonResult();
        }

        #endregion

        #region RTE Barcode

        public async Task<IActionResult> ListBarcode()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedView();

            var model = await _formatSettingModelFactory.PrepareBarcodeFormatSearchModel(new BarcodeSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ListBarcode(BarcodeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedKendoGridJson();

            var model = await _formatSettingModelFactory.PrepareBarcodeFormatListModel(searchModel);

            return Json(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> SortBarcode(string data)
        {
            data = data.Replace('"', ' ');
            Regex r = new Regex(@"Name : (.+?) , Length");
            MatchCollection mc = r.Matches(data);
            List<string> arr = new List<string>();

            foreach (Match match in mc)
            {
                foreach (Capture capture in match.Captures)
                {
                    arr.Add(capture.Value.Split(new string[] { "Name : " }, StringSplitOptions.None)[1].Split(',')[0].Trim());
                }
            }

            var barcodeFormat = await _formatSettingService.GetAllBarcodeFormatsAsync();
            foreach (var item in barcodeFormat)
            {
                for (int i = 0; i < arr.Count(); i++)
                {
                    if (item.Name == arr[i])
                    {
                        int seq = i + 1;
                        item.Sequence = seq;

                        _formatSettingService.UpdateBarcodeFormat(item);
                    }
                }
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBarcode(BarcodeModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

            bool isExist = _formatSettingService.CheckFormatExist(model.Name, null);

            if (!isExist)
            {
                var barcodeFormat = await _formatSettingService.GetBarcodeFormatByIdAsync(model.Id);
                barcodeFormat.Id = model.Id;
                barcodeFormat.Name = model.Name;

                _formatSettingService.UpdateBarcodeFormat(barcodeFormat);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Name already exist in RTE barcode format.");
                _notificationService.ErrorNotification("Name already exist in RTE barcode format.");
                return Json("Name already exist in RTE barcode format.");
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> AddBarcode(BarcodeModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError(string.Empty, "Name is required to add RTE barcode format.");
                _notificationService.ErrorNotification("Name is required to add RTE barcode format.");
            }
            try
            {
                var dataList = await _formatSettingService.GetAllBarcodeFormatsAsync();

                if (dataList.Count >= 4)
                {
                    _notificationService.WarningNotification("Add row limit to 4 only!");
                }
                else
                {
                    bool isExist = _formatSettingService.CheckFormatExist(model.Name, null);

                    if (!isExist)
                    {
                        int counter = dataList.Count;
                        FormatSetting barcodeFormat = new FormatSetting();
                        barcodeFormat.Format = "Barcode";
                        barcodeFormat.Length = Convert.ToInt32(_iconfiguration["RTEBarcodeLength"]);
                        barcodeFormat.Name = model.Name;
                        barcodeFormat.Sequence = counter + 1;

                        await _formatSettingService.InsertShelfLocationFormat(barcodeFormat); //Uses same function as shelf location
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Name already exist in RTE barcode format.");
                        _notificationService.ErrorNotification("Name already exist in RTE barcode format.");
                        return Json("Name already exist in RTE barcode format.");
                    }
                }

                return new NullJsonResult();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                _notificationService.ErrorNotification(e.Message);

                return Json(e.Message);
            }
        }

        public async Task<IActionResult> DeleteBarcode(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageFormatSetting))
                return AccessDeniedView();

            var barcodeFormat = await _formatSettingService.GetBarcodeFormatByIdAsync(id);
            if (barcodeFormat == null)
                return new NullJsonResult();

            try
            {
                _formatSettingService.DeleteShelfLocationFormat(barcodeFormat); //Uses same function as shelf location

                int? nextSeq = barcodeFormat.Sequence + 1;
                if (nextSeq > 0 && nextSeq <= 4)
                {
                    for (int? i = nextSeq; i <= 4; i++)
                    {
                        int? counter = i-1;
                        var nextBarcodeFormat = await _formatSettingService.GetBarcodeFormatBySeqAsync(i);
                        if (nextBarcodeFormat != null)
                        {
                            nextBarcodeFormat.Sequence = counter;
                            _formatSettingService.UpdateBarcodeFormat(nextBarcodeFormat);
                        }
                    }
                }

                return new NullJsonResult();
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return new NullJsonResult();
            }
        }

        #endregion

        #endregion

        #region Replenishment

        public async Task<IActionResult> Replenishment()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReplenishmentSetting))
                return AccessDeniedView();

            var model = await _replenishmentModelFactory.PrepareReplenishmentSearchModel(new ReplenishmentSearchModel());

            return View(model);
        }

        public async Task<IActionResult> GetStoreReplenishment()
        {
            var model = await _replenishmentModelFactory.PrepareReplenishmentSearchModel(new ReplenishmentSearchModel());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReplenishment(ReplenishmentModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrderLimit))
                return AccessDeniedView();

            if (model.SelectedStoreIds.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Store is required to configure replenishment");
                _notificationService.ErrorNotification("Store is required to configure replenishment");
                return new NullJsonResult();
            }

            try
            {
                Replenishment replenishment = new Replenishment
                {
                    BufferDays = model.BufferDays,
                    ReplenishmentQty = model.ReplenishmentQty,
                    ReplenishmentStores = new List<ReplenishmentStore>()
                };

                //Add store
                foreach (var store in model.SelectedStoreIds)
                {
                    ReplenishmentStore replenishmentStore = new ReplenishmentStore
                    {
                        ReplenishmentId = replenishment.Id,
                        StoreId = store
                    };

                    replenishment.ReplenishmentStores.Add(replenishmentStore);
                }

                await _replenishmentService.InsertReplenishment(replenishment);

                return new NullJsonResult();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                _notificationService.ErrorNotification(e.Message);

                return Json(e.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReplenishmentList(ReplenishmentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReplenishmentSetting))
                return AccessDeniedKendoGridJson();

            var model = await _replenishmentModelFactory.PrepareReplenishmentListModel(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> EditReplenishment(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReplenishmentSetting))
                return AccessDeniedView();

            var replenishment = await _replenishmentService.GetReplenishmentByIdAsync(id);
            if (replenishment == null)
                return RedirectToAction("Replenishment");

            var model = await _replenishmentModelFactory.PrepareReplenishmentModel(null, replenishment);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public async Task<IActionResult> EditReplenishment(ReplenishmentModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReplenishmentSetting))
                return AccessDeniedView();

            var replenishment = await _replenishmentService.GetReplenishmentByIdAsync(model.Id);
            if (replenishment == null)
                return RedirectToAction("Replenishment");

            var allStores = await _storeService.GetStoresAsync();
            var newStores = new List<Store>();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                    newStores.Add(store);
            }

            if (model.SelectedStoreIds.Count == 0)
            {
                _notificationService.ErrorNotification("Store is required");
                model = await _replenishmentModelFactory.PrepareReplenishmentModel(model, replenishment);
                model.SelectedStoreIds = new List<int>();

                return View(model);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    replenishment.BufferDays = model.BufferDays;
                    replenishment.ReplenishmentQty = model.ReplenishmentQty;

                    //stores

                    List<ReplenishmentStore> replenishmentStoreList = new List<ReplenishmentStore>();

                    foreach (var store in allStores)
                    {
                        if (model.SelectedStoreIds.Contains(store.P_BranchNo))
                        {
                            //new store
                            if (replenishment.ReplenishmentStores.Count(mapping => mapping.StoreId == store.P_BranchNo) == 0)
                            {
                                ReplenishmentStore replenishmentStore = new ReplenishmentStore
                                {
                                    ReplenishmentId = replenishment.Id,
                                    StoreId = store.P_BranchNo
                                };


                                replenishment.ReplenishmentStores.Add(replenishmentStore);
                            }
                        }
                        else
                        {
                            //remove store
                            if (replenishment.ReplenishmentStores.Count(mapping => mapping.StoreId == store.P_BranchNo) > 0)
                                _replenishmentService.DeleteReplenishmentStore(model.Id, store);
                        }
                    }

                    _replenishmentService.UpdateReplenishment(replenishment);

                    _notificationService.SuccessNotification("Replenishment has been updated successfully.");

                    if (!continueEditing)
                        return RedirectToAction("Replenishment");

                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("EditReplenishment", new { id = replenishment.Id });
                }
                catch (Exception e)
                {
                    _notificationService.ErrorNotification(e.Message);
                }
            }

            model = await _replenishmentModelFactory.PrepareReplenishmentModel(model, replenishment);

            return View(model);
        }

        public async Task<IActionResult> DeleteReplenishment(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReplenishmentSetting))
                return AccessDeniedView();

            var replenishment = await _replenishmentService.GetReplenishmentByIdAsync(id);
            if (replenishment == null)
                return RedirectToAction("Replenishment");

            try
            {
                _replenishmentService.DeleteReplenishment(replenishment);

                _notificationService.SuccessNotification("Replenishment has been deleted successfully.");

                return RedirectToAction("Replenishment");
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return RedirectToAction("EditReplenishment", new { id = replenishment.Id });
            }
        }


        #endregion

        #region General

        private void AddErrors(string result)
        {
            ModelState.AddModelError(string.Empty, result);
        }

        #endregion
    }
}