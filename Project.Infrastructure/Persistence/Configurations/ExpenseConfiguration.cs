using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.ExpenseAggregate;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses");

            builder.HasKey(e => e.Id);

            builder.HasQueryFilter(e => !e.IsDeleted);

            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Category).IsRequired().HasMaxLength(100);
            builder.Property(e => e.ExpenseDate).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(1000);

            builder.Property(e => e.ReceiptFileName).HasMaxLength(255);
            builder.Property(e => e.ReceiptFilePath).HasMaxLength(500);

            builder.OwnsOne(e => e.Amount, money =>
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
