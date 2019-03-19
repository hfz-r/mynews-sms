using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Media;

namespace StockManagementSystem.Services.Media
{
    public interface IPictureService
    {
        bool StoreInDb { get; set; }

        Task DeletePicture(Picture picture);

        string GetDefaultPictureUrl(int targetSize = 0, PictureType defaultPictureType = PictureType.Entity, string location = null);

        Task<Picture> GetPictureById(int pictureId);

        IPagedList<Picture> GetPictures(int pageIndex = 0, int pageSize = int.MaxValue);

        IDictionary<int, string> GetPicturesHash(int[] picturesIds);

        string GetPictureUrl(int pictureId, int targetSize = 0, bool showDefaultPicture = true, string location = null, PictureType defaultPictureType = PictureType.Entity);

        string GetPictureUrl(Picture picture, int targetSize = 0, bool showDefaultPicture = true, string location = null, PictureType defaultPictureType = PictureType.Entity);

        string GetThumbLocalPath(Picture picture, int targetSize = 0, bool showDefaultPicture = true);

        Task<Picture> InsertPicture(byte[] pictureBinary, string mimeType, string altAttribute = null, string titleAttribute = null, bool isNew = true, bool validateBinary = true);

        byte[] LoadPictureBinary(Picture picture);

        Task<Picture> UpdatePicture(int pictureId, byte[] pictureBinary, string mimeType, string altAttribute = null, string titleAttribute = null, bool isNew = true, bool validateBinary = true);

        byte[] ValidatePicture(byte[] pictureBinary, string mimeType);

        Task<byte[]> GetDownloadBits(IFormFile file);
    }
}