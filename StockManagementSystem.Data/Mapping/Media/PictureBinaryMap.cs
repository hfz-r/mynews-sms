using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Media;

namespace StockManagementSystem.Data.Mapping.Media
{
    public class PictureBinaryMap : EntityTypeConfiguration<PictureBinary>
    {
        public override void Configure(EntityTypeBuilder<PictureBinary> builder)
        {
            builder.ToTable(nameof(PictureBinary));
            builder.HasKey(pictureBinary => pictureBinary.Id);

            builder.HasOne(pictureBinary => pictureBinary.Picture)
                .WithOne(picture => picture.PictureBinary)
                .HasForeignKey<PictureBinary>(pictureBinary => pictureBinary.PictureId)
                .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}