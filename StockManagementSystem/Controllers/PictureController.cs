using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Media;
using StockManagementSystem.Web.Controllers;
using StockManagementSystem.Web.Mvc.Filters;

namespace StockManagementSystem.Controllers
{
    //TODO: download service
    public class PictureController : BaseController
    {
        private readonly IFileProviderHelper _fileProvider;
        private readonly IPictureService _pictureService;

        public PictureController(IFileProviderHelper fileProvider, IPictureService pictureService)
        {
            _fileProvider = fileProvider;
            _pictureService = pictureService;
        }

        #region Utilities

       
        #endregion

        [HttpPost]
        [AntiForgery(true)] //do not validate request token (XSRF)
        public async Task<IActionResult> AsyncUpload()
        {
            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = await _pictureService.GetDownloadBits(httpPostedFile);

            const string qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();

            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            //contentType is not always available 
            //that's why been manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            if (string.IsNullOrEmpty(contentType))
            {
                switch (fileExtension)
                {
                    case ".bmp":
                        contentType = MimeTypes.ImageBmp;
                        break;
                    case ".gif":
                        contentType = MimeTypes.ImageGif;
                        break;
                    case ".jpeg":
                    case ".jpg":
                    case ".jpe":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                        contentType = MimeTypes.ImageJpeg;
                        break;
                    case ".png":
                        contentType = MimeTypes.ImagePng;
                        break;
                    case ".tiff":
                    case ".tif":
                        contentType = MimeTypes.ImageTiff;
                        break;
                }
            }

            var picture = await _pictureService.InsertPicture(fileBinary, contentType);

            return Json(new { success = true, pictureId = picture.Id, imageUrl = _pictureService.GetPictureUrl(picture, 100) });
        }
    }
}