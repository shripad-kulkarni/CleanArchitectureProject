using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class MenuSettingConfiguration : IEntityTypeConfiguration<MenuSetting>
    {
        public void Configure(EntityTypeBuilder<MenuSetting> builder)
        {
            builder.ToTable("MenuSettings");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.MenuKey).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Label).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Icon).HasMaxLength(50);
            builder.Property(m => m.ParentKey).HasMaxLength(100);
            builder.Property(m => m.SortOrder).IsRequired();
            builder.Property(m => m.Role).IsRequired().HasMaxLength(50);
            builder.Property(m => m.IsVisible).IsRequired();

            builder.HasIndex(m => new { m.MenuKey, m.Role }).IsUnique();
        }
    }
}
