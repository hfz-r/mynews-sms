using System;

namespace StockManagementSystem.Core.Domain.Media
{
    public class Download : BaseEntity
    {
        public byte[] DownloadBinary { get; set; }

        public string ContentType { get; set; }

        public string Filename { get; set; }

        public string Extension { get; set; }
    }
}