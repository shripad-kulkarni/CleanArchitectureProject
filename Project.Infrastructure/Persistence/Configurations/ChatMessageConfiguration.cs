using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.ChatAggregate;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("ChatMessages");
            builder.HasKey(x => x.Id);
            builder.HasQueryFilter(x => !x.IsDeleted);

            // 128 chars is more than enough for ASP.NET Identity GUIDs (36 chars).
            // Keeping under 256 ensures the composite index stays within MySQL's 3072-byte key limit (utf8mb4).
            builder.Property(x => x.SenderId).IsRequired().HasMaxLength(128);
            builder.Property(x => x.ReceiverId).IsRequired().HasMaxLength(128);
            builder.Property(x => x.Content).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.FileUrl).HasMaxLength(1000);
            builder.Property(x => x.FileName).HasMaxLength(255);

            builder.HasIndex(x => new { x.SenderId, x.ReceiverId });
            builder.HasIndex(x => x.ReceiverId);
        }
    }
}
