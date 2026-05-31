using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.UserAggregate;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.HasQueryFilter(u => !u.IsDeleted);
            builder.Ignore(u => u.FullName);

            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.DateOfBirth).IsRequired();
            builder.Property(u => u.Gender).IsRequired();

            builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Phone).IsRequired().HasMaxLength(20);

            builder.Property(u => u.Street).IsRequired().HasMaxLength(200);
            builder.Property(u => u.City).IsRequired().HasMaxLength(100);
            builder.Property(u => u.State).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PinCode).IsRequired().HasMaxLength(20);
            builder.Property(u => u.Country).IsRequired().HasMaxLength(100);

            builder.Property(u => u.BloodGroup).HasMaxLength(10);
            builder.Property(u => u.EmergencyContact).HasMaxLength(20);
            builder.Property(u => u.Description).HasMaxLength(500);
            builder.Property(u => u.ProfilePhotoUrl).HasMaxLength(500);
            builder.Property(u => u.IntroVideoUrl).HasMaxLength(500);

            builder.HasMany(u => u.Documents)
                .WithOne()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(u => u.Documents)
                .HasField("_documents")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public sealed class UserDocumentConfiguration : IEntityTypeConfiguration<UserDocument>
    {
        public void Configure(EntityTypeBuilder<UserDocument> builder)
        {
            builder.ToTable("UserDocuments");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FileName).IsRequired().HasMaxLength(255);
            builder.Property(d => d.FilePath).IsRequired().HasMaxLength(500);
            builder.Property(d => d.DocumentType).IsRequired();
        }
    }
}
