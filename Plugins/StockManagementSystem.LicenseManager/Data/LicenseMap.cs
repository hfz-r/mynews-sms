using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Data.Mapping;
using StockManagementSystem.LicenseManager.Domain;

namespace StockManagementSystem.LicenseManager.Data
{
    public class LicenseMap : EntityTypeConfiguration<License>
    {
        public override void Configure(EntityTypeBuilder<License> builder)
        {
            builder.ToTable(nameof(License));
            builder.HasKey(l => l.Id);

            builder.Property(l => l.PublicKey).HasMaxLength(Int32.MaxValue);
            builder.Property(l => l.PrivateKey).HasMaxLength(Int32.MaxValue);
            builder.Property(l => l.LicenseToName).IsRequired();
            builder.Property(l => l.LicenseToEmail).IsRequired();
            builder.Property(l => l.ExpirationDate).IsRequired();

            builder.Ignore(l => l.LicenseId);
            builder.Ignore(l => l.PassPhrase);
            builder.Ignore(l => l.LicenseType);
            builder.Ignore(l => l.Quantity);
            builder.Ignore(l => l.ProductFeatures);
        }
    }
}