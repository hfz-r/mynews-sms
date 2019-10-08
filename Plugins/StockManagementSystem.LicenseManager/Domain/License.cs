using System;
using System.Collections.Generic;
using Portable.Licensing;
using StockManagementSystem.Core;

namespace StockManagementSystem.LicenseManager.Domain
{
    public class License : BaseEntity
    {
        public License()
        {
            LicenseId = Guid.NewGuid();
            PassPhrase = Guid.NewGuid().ToString();
        }

        public Guid LicenseId { get; set; }

        public string PassPhrase { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        public string LicenseToName { get; set; }

        public string LicenseToEmail { get; set; }

        public int LicenseTypeId { get; set; }

        public LicenseType LicenseType
        {
            get => (LicenseType)LicenseTypeId;
            set => LicenseTypeId = (int)value;
        }

        public DateTime ExpirationDate { get; set; }

        public int Quantity { get; set; }

        public int DownloadId { get; set; }

        public IDictionary<string, string> ProductFeatures { get; set; }
    }
}