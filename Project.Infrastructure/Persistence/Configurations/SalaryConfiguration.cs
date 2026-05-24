using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.SalaryAggregate;
using Project.Domain.Aggregates.StaffAggregate;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class SalaryConfiguration : IEntityTypeConfiguration<Salary>
    {
        public void Configure(EntityTypeBuilder<Salary> builder)
        {
            builder.ToTable("Salaries");

            builder.HasKey(s => s.Id);

            builder.HasQueryFilter(s => !s.IsDeleted);

            builder.Property(s => s.StaffId).IsRequired();
            builder.Property(s => s.Month).IsRequired();
            builder.Property(s => s.Year).IsRequired();
            builder.Property(s => s.Status).IsRequired();

            builder.HasIndex(s => new { s.StaffId, s.Month, s.Year }).IsUnique();

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

            builder.OwnsOne(s => s.Allowances, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("AllowancesAmount")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency)
                    .HasColumnName("AllowancesCurrency")
                    .IsRequired()
                    .HasMaxLength(10);
            });

            builder.OwnsOne(s => s.Deductions, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("DeductionsAmount")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency)
                    .HasColumnName("DeductionsCurrency")
                    .IsRequired()
                    .HasMaxLength(10);
            });

            builder.OwnsOne(s => s.NetSalary, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("NetSalaryAmount")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency)
                    .HasColumnName("NetSalaryCurrency")
                    .IsRequired()
                    .HasMaxLength(10);
            });

            builder.HasOne<Staff>()
                .WithMany()
                .HasForeignKey(s => s.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
