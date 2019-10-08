using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Media;

namespace StockManagementSystem.Data.Mapping.Media
{
    public class DownloadMap : EntityTypeConfiguration<Download>
    {
        public override void Configure(EntityTypeBuilder<Download> builder)
        {
            builder.ToTable(nameof(Download));
            builder.HasKey(download => download.Id);

            base.Configure(builder);
        }
    }
}