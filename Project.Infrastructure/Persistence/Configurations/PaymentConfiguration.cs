using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.PaymentAggregate;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(p => p.Id);
            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.Property(p => p.GatewayOrderId).IsRequired().HasMaxLength(100);
            builder.Property(p => p.GatewayPaymentId).HasMaxLength(100);
            builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.Currency).IsRequired().HasMaxLength(10);
            builder.Property(p => p.Receipt).HasMaxLength(100);
            builder.Property(p => p.Status).IsRequired();
            builder.Property(p => p.Notes).HasMaxLength(500);

            builder.HasIndex(p => p.GatewayOrderId).IsUnique();
        }
    }
}
