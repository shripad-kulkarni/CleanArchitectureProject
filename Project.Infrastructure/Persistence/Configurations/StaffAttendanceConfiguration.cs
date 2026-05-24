using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.StaffAggregate;
using Project.Domain.Aggregates.StaffAttendanceAggregates;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class StaffAttendanceConfiguration : IEntityTypeConfiguration<StaffAttendance>
    {
        public void Configure(EntityTypeBuilder<StaffAttendance> builder)
        {
            builder.ToTable("StaffAttendances");

            builder.HasKey(a => a.Id);

            builder.HasQueryFilter(a => !a.IsDeleted);

            builder.Property(a => a.StaffId).IsRequired();
            builder.Property(a => a.Date).IsRequired();
            builder.Property(a => a.Status).IsRequired();
            builder.Property(a => a.Remarks).HasMaxLength(500);

            builder.HasIndex(a => new { a.StaffId, a.Date }).IsUnique();

            builder.HasOne<Staff>()
                .WithMany()
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
