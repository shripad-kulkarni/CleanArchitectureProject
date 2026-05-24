using Project.Domain.Enums;
using Project.Domain.Primitives;
using Project.Domain.ValueObjects;

namespace Project.Domain.Aggregates.StudentAggregate
{
    public sealed class Student : AggregateRoot
    {
        private readonly List<StudentDocument> _documents = [];

        private Student() { }

        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public Email Email { get; private set; } = null!;
        public PhoneNumber Phone { get; private set; } = null!;
        public DateOnly DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public Address Address { get; private set; } = null!;
        public string AdmissionNumber { get; private set; } = string.Empty;
        public DateOnly AdmissionDate { get; private set; }
        public string RollNumber { get; private set; } = string.Empty;
        public string ClassName { get; private set; } = string.Empty;
        public string AcademicYear { get; private set; } = string.Empty;

        // Profile fields (formerly StudentProfile entity)
        public string? BloodGroup { get; private set; }
        public string? ParentName { get; private set; }
        public string? ParentPhone { get; private set; }
        public string? ParentEmail { get; private set; }
        public string? EmergencyContact { get; private set; }

        public IReadOnlyCollection<StudentDocument> Documents => _documents.AsReadOnly();

        public static Student Create(
            string firstName,
            string lastName,
            Email email,
            PhoneNumber phone,
            DateOnly dateOfBirth,
            Gender gender,
            Address address,
            string admissionNumber,
            DateOnly admissionDate,
            string rollNumber,
            string className,
            string academicYear,
            string? bloodGroup = null,
            string? parentName = null,
            string? parentPhone = null,
            string? parentEmail = null,
            string? emergencyContact = null)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");
            if (string.IsNullOrWhiteSpace(admissionNumber)) throw new ArgumentException("Admission number is required.");
            if (string.IsNullOrWhiteSpace(rollNumber)) throw new ArgumentException("Roll number is required.");
            if (string.IsNullOrWhiteSpace(className)) throw new ArgumentException("Class name is required.");
            if (string.IsNullOrWhiteSpace(academicYear)) throw new ArgumentException("Academic year is required.");

            return new Student
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                Address = address,
                AdmissionNumber = admissionNumber,
                AdmissionDate = admissionDate,
                RollNumber = rollNumber,
                ClassName = className,
                AcademicYear = academicYear,
                BloodGroup = bloodGroup,
                ParentName = parentName,
                ParentPhone = parentPhone,
                ParentEmail = parentEmail,
                EmergencyContact = emergencyContact
            };
        }

        public void Update(
            string firstName,
            string lastName,
            PhoneNumber phone,
            Address address,
            string rollNumber,
            string className,
            string academicYear)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");

            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Address = address;
            RollNumber = rollNumber;
            ClassName = className;
            AcademicYear = academicYear;
        }

        public void UpdateProfile(
            string? bloodGroup,
            string? parentName,
            string? parentPhone,
            string? parentEmail,
            string? emergencyContact)
        {
            BloodGroup = bloodGroup;
            ParentName = parentName;
            ParentPhone = parentPhone;
            ParentEmail = parentEmail;
            EmergencyContact = emergencyContact;
        }

        public void AddDocument(StudentDocument document)
        {
            ArgumentNullException.ThrowIfNull(document);
            _documents.Add(document);
        }
    }
}
