using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.FeeAggregate;
using Project.Domain.Aggregates.StudentAggregate;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class FeeConfiguration : IEntityTypeConfiguration<Fee>
    {
        public void Configure(EntityTypeBuilder<Fee> builder)
        {
            builder.ToTable("Fees");

            builder.HasKey(f => f.Id);

            builder.HasQueryFilter(f => !f.IsDeleted);

            builder.Property(f => f.StudentId).IsRequired();
            builder.Property(f => f.FeeName).IsRequired().HasMaxLength(200);
            builder.Property(f => f.DueDate).IsRequired();
            builder.Property(f => f.TotalInstallments).IsRequired();
            builder.Property(f => f.Status).IsRequired();

            builder.Property(f => f.PaymentMode).HasMaxLength(50);
            builder.Property(f => f.TransactionReference).HasMaxLength(200);

            builder.Ignore(f => f.RemainingAmount);

            builder.OwnsOne(f => f.TotalAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("TotalAmount")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency)
                    .HasColumnName("TotalAmountCurrency")
                    .IsRequired()
                    .HasMaxLength(10);
            });

            builder.OwnsOne(f => f.PaidAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("PaidAmount")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency)
                    .HasColumnName("PaidAmountCurrency")
                    .IsRequired()
                    .HasMaxLength(10);
            });

            builder.HasOne<Student>()
                .WithMany()
                .HasForeignKey(f => f.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(f => f.Installments)
                .WithOne()
                .HasForeignKey(i => i.FeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(f => f.Installments)
                .HasField("_installments")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public sealed class FeeInstallmentConfiguration : IEntityTypeConfiguration<FeeInstallment>
    {
        public void Configure(EntityTypeBuilder<FeeInstallment> builder)
        {
            builder.ToTable("FeeInstallments");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.FeeId).IsRequired();
            builder.Property(i => i.InstallmentNumber).IsRequired();
            builder.Property(i => i.DueDate).IsRequired();
            builder.Property(i => i.Status).IsRequired();

            builder.HasIndex(i => new { i.FeeId, i.InstallmentNumber }).IsUnique();

            builder.OwnsOne(i => i.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Amount")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency)
                    .HasColumnName("AmountCurrency")
                    .IsRequired()
                    .HasMaxLength(10);
            });
        }
    }
}
