using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Models.Setting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StockManagementSystem.Controllers
{
    public class SettingController : Controller
    {
        private bool _disposed;

        private readonly IRepository<Approval> _approvalRepository;
        private readonly IRepository<OrderLimit> _orderLimitRepository;
        private readonly IRepository<OrderLimitStore> _orderLimitStoreRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly ILogger _logger;

        #region Constructor

        public SettingController(
            IRepository<Approval> approvalRepository,
            IRepository<OrderLimit> orderLimitRepository,
            IRepository<OrderLimitStore> orderLimitStoreRepository,
            IRepository<Store> storeRepository,
            ILoggerFactory loggerFactory)
        {
            this._approvalRepository = approvalRepository;
            this._orderLimitRepository = orderLimitRepository;
            this._orderLimitStoreRepository = orderLimitStoreRepository;
            this._storeRepository = storeRepository;
            _logger = loggerFactory.CreateLogger<SettingController>();

        }

        #endregion

        #region Destructor

        ~SettingController()
        {
            Dispose(false);
        }

        #endregion

        #region IDisposable 

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Stock Order

        [HttpGet]
        public IActionResult Order()
        {
            return View("Order", GetAllOrderLimit());
        }

        [HttpGet]
        public IActionResult AddOrder()
        {
            return View("AddOrder", GetAllStore());
        }

        // POST: /Order/AddOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveOrderLimit(SettingViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!OrderLimitExist(model))
                    {
                        var orderLimit = new OrderLimit
                        {
                            Percentage = model.Percentage,
                            CreatedBy = @Environment.UserName,
                            CreatedOn = DateTime.UtcNow, //TODO Change to get server datetime
                            ModifiedBy = @Environment.UserName,
                            ModifiedOn = DateTime.UtcNow //TODO Change to get server datetime
                        };

                        _orderLimitRepository.Insert(orderLimit);

                        var orderLimitStore = new OrderLimitStore
                        {
                            StoreId = model.P_BranchNo,
                            OrderLimitId = orderLimit.Id,
                            CreatedBy = @Environment.UserName,
                            CreatedOn = DateTime.UtcNow, //TODO Change to get server datetime
                            ModifiedBy = @Environment.UserName,
                            ModifiedOn = DateTime.UtcNow //TODO Change to get server datetime

                        };

                        _orderLimitStoreRepository.Insert(orderLimitStore);

                        _logger.LogInformation(3, "Order Limit" + " created successfully.");
                        return RedirectToAction("Order");
                    }
                    else
                    {
                        return View("AddOrder", model);
                    }
                }

                // If we got this far, something failed, redisplay form
                return View("AddOrder", model);
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
            }

            return View("AddOrder", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrderLimit(int? id)
        {
            return View("EditOrder", GetOrderLimit(id));
        }

        // POST: /Device/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveEditedOrderLimit(int id, SettingViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var orderLimit = _orderLimitRepository.Table.FirstOrDefault(x => x.Id == model.OrderLimitId);
                    orderLimit.Percentage = model.Percentage;
                    orderLimit.ModifiedBy = @Environment.UserName;
                    orderLimit.ModifiedOn = DateTime.UtcNow; //TODO Change to get server datetime                    

                    _orderLimitRepository.Update(orderLimit);

                    var orderLimitStore = _orderLimitStoreRepository.Table.FirstOrDefault(x => x.OrderLimitId == model.OrderLimitId);
                    orderLimitStore.StoreId = model.P_BranchNo;

                    _orderLimitStoreRepository.Update(orderLimitStore);

                    _logger.LogInformation(3, "Order Limit" + " edited successfully.");
                    return RedirectToAction("Order");
                }

                // If we got this far, something failed, redisplay form
                return View("RegisterDevice", model);
            }
            catch (Exception ex)
            {
                AddErrors(ex.Message);
            }

            return View("RegisterDevice", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOrderLimit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            OrderLimitStore orderLimitStore = _orderLimitStoreRepository.Table.FirstOrDefault(x => x.OrderLimitId == id);
            _orderLimitStoreRepository.Delete(orderLimitStore);

            if (orderLimitStore == null)
            {
                return NotFound();
            }

            OrderLimit orderLimit = _orderLimitRepository.GetById(id);
            _orderLimitRepository.Delete(orderLimit);

            if (orderLimit == null)
            {
                return NotFound();
            }

            return RedirectToAction("Order");
        }

        private bool OrderLimitExist(SettingViewModel model)
        {
            //TODO
            //if (_orderLimitRepository.Table.Any(x => x.ModelNo == model.ModelNo && x.SerialNo == model.SerialNo))
            //    return true;
            //else
                return false;
        }

        private SettingViewModel GetOrderLimit(int? id)
        {
            OrderLimit orderLimit = _orderLimitRepository.GetById(id);
            ICollection<OrderLimit> orderLimits = new ObservableCollection<OrderLimit>();
            orderLimits.Add(orderLimit);

            OrderLimitStore orderLimitStore = _orderLimitStoreRepository.Table.FirstOrDefault(x => x.OrderLimitId == id);

            SettingViewModel model = new SettingViewModel
            {
                OrderLimitId = id,
                Percentage = orderLimit.Percentage,
                P_BranchNo = orderLimitStore.StoreId,
                Store = _storeRepository.Table.OrderBy(x => x.P_Name).ToList()
            };

            return model;
        }

        private SettingViewModel GetAllOrderLimit()
        {
            SettingViewModel model = new SettingViewModel
            {
                OrderLimit = _orderLimitRepository.Table.ToList()
            };

            return model;
        }

        private SettingViewModel GetAllStore()
        {
            SettingViewModel model = new SettingViewModel
            {
                Store = _storeRepository.Table.OrderBy(x => x.P_Name).ToList()
            };

            return model;
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

        [HttpGet]
        public IActionResult Location()
        {
            return View("Location");
        }

        #endregion

        private void AddErrors(string result)
        {
            ModelState.AddModelError(string.Empty, result);
        }
    }
}