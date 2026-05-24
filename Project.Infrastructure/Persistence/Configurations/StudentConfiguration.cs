using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Aggregates.StudentAggregate;

namespace Project.Infrastructure.Persistence.Configurations
{
    public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");

            builder.HasKey(s => s.Id);

            builder.HasQueryFilter(s => !s.IsDeleted);

            builder.Ignore(s => s.FullName);

            builder.Property(s => s.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(s => s.LastName).IsRequired().HasMaxLength(100);
            builder.Property(s => s.AdmissionNumber).IsRequired().HasMaxLength(50);
            builder.Property(s => s.RollNumber).IsRequired().HasMaxLength(50);
            builder.Property(s => s.ClassName).IsRequired().HasMaxLength(100);
            builder.Property(s => s.AcademicYear).IsRequired().HasMaxLength(20);
            builder.Property(s => s.DateOfBirth).IsRequired();
            builder.Property(s => s.AdmissionDate).IsRequired();
            builder.Property(s => s.Gender).IsRequired();

            builder.Property(s => s.BloodGroup).HasMaxLength(10);
            builder.Property(s => s.ParentName).HasMaxLength(200);
            builder.Property(s => s.ParentPhone).HasMaxLength(20);
            builder.Property(s => s.ParentEmail).HasMaxLength(200);
            builder.Property(s => s.EmergencyContact).HasMaxLength(20);

            builder.HasIndex(s => s.AdmissionNumber).IsUnique();

            builder.OwnsOne(s => s.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(200);
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

            builder.HasMany(s => s.Documents)
                .WithOne()
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(s => s.Documents)
                .HasField("_documents")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }

    public sealed class StudentDocumentConfiguration : IEntityTypeConfiguration<StudentDocument>
    {
        public void Configure(EntityTypeBuilder<StudentDocument> builder)
        {
            builder.ToTable("StudentDocuments");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FileName).IsRequired().HasMaxLength(255);
            builder.Property(d => d.FilePath).IsRequired().HasMaxLength(500);
            builder.Property(d => d.DocumentType).IsRequired();
        }
    }
}
