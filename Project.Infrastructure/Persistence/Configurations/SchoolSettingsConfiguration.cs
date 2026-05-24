using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.SchoolSettingAggregate;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class SchoolSettingsConfiguration : IEntityTypeConfiguration<SchoolSettings>
    {
        public void Configure(EntityTypeBuilder<SchoolSettings> builder)
        {
            builder.ToTable("SchoolSettings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SchoolName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.LogoPath).HasMaxLength(500);
            builder.Property(x => x.Address).HasMaxLength(500);
            builder.Property(x => x.PhoneNumber).HasMaxLength(30);
            builder.Property(x => x.Email).HasMaxLength(150);
        }
    }
}
