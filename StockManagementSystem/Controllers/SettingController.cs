using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Approvals;
using System;

namespace StockManagementSystem.Controllers
{
    public class SettingController : Controller
    {
        private bool _disposed;

        private readonly IRepository<Approval> _approvalRepository;
        private readonly ILogger _logger;

        #region Constructor

        public SettingController(
            IRepository<Approval> approvalRepository,
            ILoggerFactory loggerFactory)
        {
            this._approvalRepository = approvalRepository;
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
            return View("Order");
        }

        [HttpGet]
        public IActionResult AddOrder()
        {
            return View("AddOrder");
        }

        #endregion

        #region Approval

        [HttpGet]
        public IActionResult Approval()
        {
            return View("Approval");
        }

        // POST: /Approval/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(SettingViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!DeviceExist(model))
                    {
                        var device = new Approval
                        {
                            IsApprovalEnabled = , //Inactive
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
            catch (Exception ex)
            {
                AddErrors(ex.Message);
            }

            return View("RegisterDevice", model);
        }

        #endregion

        #region Location

        [HttpGet]
        public IActionResult Location()
        {
            return View("Location");
        }

        #endregion
    }
}