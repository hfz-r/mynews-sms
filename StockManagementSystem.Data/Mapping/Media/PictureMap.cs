using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Media;

namespace StockManagementSystem.Data.Mapping.Media
{
    public class PictureMap : EntityTypeConfiguration<Picture>
    {
        public override void Configure(EntityTypeBuilder<Picture> builder)
        {
            builder.ToTable(nameof(Picture));
            builder.HasKey(picture => picture.Id);

            builder.Property(picture => picture.MimeType).HasMaxLength(40).IsRequired();

            base.Configure(builder);
        }
    }
}