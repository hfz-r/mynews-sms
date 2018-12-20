using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Models.Devices;
using System;
using System.Linq;

namespace StockManagementSystem.Controllers
{
    public class DeviceController : Controller
    {
        private bool _disposed;

        private readonly IRepository<Device> _deviceRepository;

        #region Constructor

        public DeviceController(IRepository<Device> deviceRepository)
        {
            this._deviceRepository = deviceRepository;
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

        [HttpGet]
        public IActionResult Index()
        {
            DeviceViewModel model = new DeviceViewModel();
            model.Device = _deviceRepository.Table.Select(x => new Device
            {
                Id = x.Id,
                TokenId = x.TokenId,
                SerialNo = x.SerialNo,
                ModelNo = x.ModelNo,
                Longitude = x.Longitude,
                Latitude = x.Latitude,
                StoreId = "TEST", //TODO
                Status = x.Status == "1" ? "Active" : "Inactive",
                CreatedBy = x.CreatedBy,
                CreatedOn = Convert.ToDateTime(x.CreatedOn.ToString()),
                ModifiedBy = x.ModifiedBy,
                ModifiedOn = Convert.ToDateTime(x.ModifiedOn.ToString())
            }).ToList();

            return View("Device", model);
        }



        [HttpGet]
        public IActionResult RegisterDevice()
        {
            return View("RegisterDevice");
        }
    }
}