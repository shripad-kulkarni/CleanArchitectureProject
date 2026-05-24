using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.SalaryAggregate;
using Project.Domain.Aggregates.StaffAggregate;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.ToTable("Staffs");

            builder.HasKey(s => s.Id);

            builder.HasQueryFilter(s => !s.IsDeleted);

            builder.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(s => s.LastName).IsRequired().HasMaxLength(100);
            builder.Property(s => s.EmployeeCode).IsRequired().HasMaxLength(50);
            builder.Property(s => s.Role).IsRequired();
            builder.Property(s => s.DateOfBirth).IsRequired();
            builder.Property(s => s.JoiningDate).IsRequired();
            builder.Property(s => s.Gender).IsRequired();

            builder.Ignore(s => s.FullName);

            builder.HasIndex(s => s.EmployeeCode).IsUnique();

            builder.OwnsOne(s => s.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(200);

                email.HasIndex(e => e.Value).IsUnique();
            });

            builder.OwnsOne(s => s.Phone, phone =>
            {
                phone.Property(p => p.Value)
                    .HasColumnName("Phone")
                    .IsRequired()
                    .HasMaxLength(20);
            });

            builder.OwnsOne(s => s.Address, address =>
            {
                address.Property(a => a.Street).HasColumnName("Street").IsRequired().HasMaxLength(200);
                address.Property(a => a.City).HasColumnName("City").IsRequired().HasMaxLength(100);
                address.Property(a => a.State).HasColumnName("State").IsRequired().HasMaxLength(100);
                address.Property(a => a.PinCode).HasColumnName("PinCode").IsRequired().HasMaxLength(20);
                address.Property(a => a.Country).HasColumnName("Country").IsRequired().HasMaxLength(100);
            });

            builder.OwnsOne(s => s.BasicSalary, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("BasicSalaryAmount")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency)
                    .HasColumnName("BasicSalaryCurrency")
                    .IsRequired()
                    .HasMaxLength(10);
            });

            // SalaryIncrement is a true child entity — owned via navigation
            builder.HasMany(s => s.SalaryIncrements)
                .WithOne()
                .HasForeignKey(si => si.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public sealed class SalaryIncrementConfiguration : IEntityTypeConfiguration<SalaryIncrement>
    {
        public void Configure(EntityTypeBuilder<SalaryIncrement> builder)
        {
            builder.ToTable("SalaryIncrements");

            builder.HasKey(si => si.Id);

            builder.Property(si => si.StaffId).IsRequired();
            builder.Property(si => si.Reason).IsRequired().HasMaxLength(1000);
            builder.Property(si => si.EffectiveDate).IsRequired();

            builder.OwnsOne(si => si.PreviousSalary, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("PreviousSalaryAmount")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency)
                    .HasColumnName("PreviousSalaryCurrency")
                    .IsRequired()
                    .HasMaxLength(10);
            });

            builder.OwnsOne(si => si.NewSalary, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("NewSalaryAmount")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency)
                    .HasColumnName("NewSalaryCurrency")
                    .IsRequired()
                    .HasMaxLength(10);
            });
        }
    }
}
