using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Models.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StockManagementSystem.Controllers
{
    public class DeviceController : Controller
    {
        private bool _disposed;

        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly ILogger _logger;

        #region Constructor

        public DeviceController(
            IRepository<Device> deviceRepository,
            IRepository<Store> storeRepository,
            ILoggerFactory loggerFactory)
        {
            this._deviceRepository = deviceRepository;
            this._storeRepository = storeRepository;
            _logger = loggerFactory.CreateLogger<DeviceController>();

        }

        #endregion

        #region Destructor

        ~DeviceController()
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

        #region Manage Device

        [HttpGet]
        public IActionResult Index()
        {
            return View("Device", GetAllDevice());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Device device = _deviceRepository.GetById(id);
            _deviceRepository.Delete(device);

            if (device == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult RegisterDevice()
        {
            return View("RegisterDevice", GetAllStore());
        }
        
        // POST: /Device/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(DeviceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!DeviceExist(model))
                    {
                        var device = new Device
                        {
                            SerialNo = model.SerialNo,
                            ModelNo = model.ModelNo,
                            StoreId = model.P_BranchNo,
                            Status = "ACTIVE",
                            EndDate = null,
                            StartDate = null,
                            CreatedBy = @Environment.UserName,
                            CreatedOn = DateTime.UtcNow, //TODO Change to get server datetime
                            ModifiedBy = @Environment.UserName,
                            ModifiedOn = DateTime.UtcNow //TODO Change to get server datetime
                        };

                        _deviceRepository.Insert(device);
                        _logger.LogInformation(3, "Device(" + device.SerialNo + ") created successfully.");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View("RegisterDevice", model);
                    }
                }

                // If we got this far, something failed, redisplay form
                return View("RegisterDevice", model);
            }
            catch(Exception ex)
            {
                AddErrors(ex.Message);
            }

            return View("RegisterDevice", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDevice(int? id)
        {
            return View("EditDevice", GetDevice(id));
        }

        // POST: /Device/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DeviceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var device = _deviceRepository.Table.FirstOrDefault(x => x.Id == model.DeviceId);
                    device.SerialNo = model.SerialNo;
                    device.ModelNo = model.ModelNo;
                    device.StoreId = model.P_BranchNo;
                    device.Status = "ACTIVE"; //TODO
                    device.ModifiedBy = @Environment.UserName;
                    device.ModifiedOn = DateTime.UtcNow; //TODO Change to get server datetime                    

                    _deviceRepository.Update(device);
                    _logger.LogInformation(3, "Device(" + device.SerialNo + ") edited successfully.");
                    return RedirectToAction("Index");
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

        #endregion

        #region Device Tracking

        [HttpGet]
        public IActionResult DeviceTracking()
        {
            return View("DeviceTracking");
        }
        #endregion

        #region Private Method

        private void AddErrors(string result)
        {
            ModelState.AddModelError(string.Empty, result);
        }

        private DeviceViewModel GetAllDevice()
        {
            DeviceViewModel model = new DeviceViewModel
            {
                Device = _deviceRepository.Table.ToList()
            };

            return model;
        }

        private DeviceViewModel GetAllStore()
        {
            DeviceViewModel model = new DeviceViewModel
            {
                Store = _storeRepository.Table.OrderBy(x => x.P_Name).ToList()
            };

            return model;
        }

        private DeviceViewModel GetDevice(int? id)
        {
            Device device = _deviceRepository.GetById(id);
            ICollection<Device> devices = new ObservableCollection<Device>();
            devices.Add(device);
            
            DeviceViewModel model = new DeviceViewModel
            {
                DeviceId = id,
                ModelNo = device.ModelNo,
                P_BranchNo = device.StoreId,
                SerialNo = device.SerialNo,
                Status = device.Status,
                Device = devices,
                Store = _storeRepository.Table.OrderBy(x => x.P_Name).ToList()
            };

            return model;
        }

        private bool DeviceExist(DeviceViewModel model)
        {
            if (_deviceRepository.Table.Any(x => x.ModelNo == model.ModelNo && x.SerialNo == model.SerialNo))
                return true;
            else
                return false;
        }

        #endregion  
    }
}