using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Directory;

namespace StockManagementSystem.Data.Mapping.Directory
{
    public partial class LocalStateMap : EntityTypeConfiguration<LocalState>
    {
        public override void Configure(EntityTypeBuilder<LocalState> builder)
        {
            //builder.ToTable(nameof(LocalState));
            //builder.HasKey(state => state.Abbreviation);

            //builder.Ignore(state => state.Id);

            //builder.Property(state => state.Abbreviation).HasMaxLength(3);
            //builder.Property(state => state.Description).HasMaxLength(100);

            //builder.HasMany(holiday => holiday.Holidays)
            //    .WithOne(state => state.State)
            //    .HasPrincipalKey(state => state.Abbreviation)
            //    .IsRequired();
          
            //base.Configure(builder);
        }
    }
}