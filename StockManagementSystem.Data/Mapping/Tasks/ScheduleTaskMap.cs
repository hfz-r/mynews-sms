using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Tasks;

namespace StockManagementSystem.Data.Mapping.Tasks
{
    public class ScheduleTaskMap : EntityTypeConfiguration<ScheduleTask>
    {
        public override void Configure(EntityTypeBuilder<ScheduleTask> builder)
        {
            builder.ToTable(nameof(ScheduleTask));
            builder.HasKey(task => task.Id);

            builder.Property(task => task.Name).IsRequired();
            builder.Property(task => task.Type).IsRequired();

            base.Configure(builder);
        }
    }
}