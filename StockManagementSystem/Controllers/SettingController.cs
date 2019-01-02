using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Settings;
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