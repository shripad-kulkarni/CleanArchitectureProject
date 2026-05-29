using Project.Domain.Enums;
using Project.Domain.Primitives;
using Project.Domain.ValueObjects;

namespace Project.Domain.Aggregates.UserAggregate
{
    public sealed class User : AggregateRoot
    {
        private readonly List<UserDocument> _documents = [];

        private User() { }

        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public Email Email { get; private set; } = null!;
        public PhoneNumber Phone { get; private set; } = null!;
        public DateOnly DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public Address Address { get; private set; } = null!;

        public string? BloodGroup { get; private set; }
        public string? EmergencyContact { get; private set; }

        public IReadOnlyCollection<UserDocument> Documents => _documents.AsReadOnly();

        public static User Create(
            string firstName,
            string lastName,
            Email email,
            PhoneNumber phone,
            DateOnly dateOfBirth,
            Gender gender,
            Address address,
            string? bloodGroup = null,
            string? emergencyContact = null)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");

            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                Address = address,
                BloodGroup = bloodGroup,
                EmergencyContact = emergencyContact
            };
        }

        public void Update(
            string firstName,
            string lastName,
            PhoneNumber phone,
            Address address)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");

            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Address = address;
        }

        public void UpdateProfile(string? bloodGroup, string? emergencyContact)
        {
            BloodGroup = bloodGroup;
            EmergencyContact = emergencyContact;
        }

        public void AddDocument(UserDocument document)
        {
            ArgumentNullException.ThrowIfNull(document);
            _documents.Add(document);
        }
    }
}
