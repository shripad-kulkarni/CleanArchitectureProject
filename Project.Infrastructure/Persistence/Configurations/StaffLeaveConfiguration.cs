using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.StaffAggregate;
using Project.Domain.Aggregates.StaffLeaveAggregate;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class StaffLeaveConfiguration : IEntityTypeConfiguration<StaffLeave>
    {
        public void Configure(EntityTypeBuilder<StaffLeave> builder)
        {
            builder.ToTable("StaffLeaves");

            builder.HasKey(l => l.Id);

            builder.HasQueryFilter(l => !l.IsDeleted);

            builder.Property(l => l.StaffId).IsRequired();
            builder.Property(l => l.LeaveType).IsRequired();
            builder.Property(l => l.Reason).IsRequired().HasMaxLength(1000);
            builder.Property(l => l.Status).IsRequired();
            builder.Property(l => l.RejectionReason).HasMaxLength(1000);

            builder.OwnsOne(l => l.DateRange, dr =>
            {
                dr.Property(d => d.StartDate).HasColumnName("LeaveStartDate").IsRequired();
                dr.Property(d => d.EndDate).HasColumnName("LeaveEndDate").IsRequired();
            });

            builder.HasOne<Staff>()
                .WithMany()
                .HasForeignKey(l => l.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
