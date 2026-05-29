using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class InfoSettingsConfiguration : IEntityTypeConfiguration<InfoSetting>
    {
        public void Configure(EntityTypeBuilder<InfoSetting> builder)
        {
            builder.ToTable("SchoolSettings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.LogoPath).HasMaxLength(500);
            builder.Property(x => x.Address).HasMaxLength(500);
            builder.Property(x => x.PhoneNumber).HasMaxLength(30);
            builder.Property(x => x.Email).HasMaxLength(150);
        }
    }
}
