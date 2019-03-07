using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Data.Mapping.Stores
{
    public partial class UserStoreMap : EntityTypeConfiguration<UserStore>
    {
        public override void Configure(EntityTypeBuilder<UserStore> builder)
        {
            builder.ToTable(nameof(UserStore));
            builder.HasKey(mapping => new { mapping.StoreId, mapping.UserId });

            builder.Property(mapping => mapping.StoreId).HasColumnName("Store_Id");
            builder.Property(mapping => mapping.UserId).HasColumnName("User_Id");
          
            builder.HasOne(mapping => mapping.User)
                .WithMany(user => user.UserStores)
                .HasForeignKey(mapping => mapping.UserId)
                .IsRequired();

            builder.HasOne(mapping => mapping.Store)
                .WithMany(store => store.UserStores)
                .HasForeignKey(mapping => mapping.StoreId)
                .IsRequired();

            builder.Ignore(mapping => mapping.Id);

            base.Configure(builder);
        }
    }
}