using Project.Domain.Entities;
using Project.Domain.Enums;
using Project.Domain.Primitives;

namespace Project.Domain.Aggregates.UserAggregate
{
    public sealed class User : AggregateRoot
    {
        private readonly List<UserDocument> _documents = [];

        private User() { }

        public User(
            string firstName,
            string lastName,
            string email,
            string phone,
            DateOnly dateOfBirth,
            Gender gender,
            string street,
            string city,
            string state,
            string pinCode,
            string country = "India",
            string? bloodGroup = null,
            string? emergencyContact = null,
            string? description = null)
        {
            FirstName        = firstName;
            LastName         = lastName;
            Email            = email.ToLowerInvariant();
            Phone            = phone;
            DateOfBirth      = dateOfBirth;
            Gender           = gender;
            Street           = street;
            City             = city;
            State            = state;
            PinCode          = pinCode;
            Country          = country;
            BloodGroup       = bloodGroup;
            EmergencyContact = emergencyContact;
            Description      = description;
        }

        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public DateOnly DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public string Street { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string PinCode { get; private set; } = string.Empty;
        public string Country { get; private set; } = "India";

        public string? BloodGroup { get; private set; }
        public string? EmergencyContact { get; private set; }
        public string? Description { get; private set; }
        public string? ProfilePhotoUrl { get; private set; }
        public string? IntroVideoUrl { get; private set; }

        public IReadOnlyCollection<UserDocument> Documents => _documents.AsReadOnly();

        public void Update(
            string firstName,
            string lastName,
            string phone,
            string street,
            string city,
            string state,
            string pinCode,
            string? description = null)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");

            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Street = street;
            City = city;
            State = state;
            PinCode = pinCode;
            Description = description;
        }

        public void UpdateProfile(string? bloodGroup, string? emergencyContact)
        {
            BloodGroup = bloodGroup;
            EmergencyContact = emergencyContact;
        }

        public void SetProfilePhotoUrl(string? url) => ProfilePhotoUrl = url;
        public void SetIntroVideoUrl(string? url) => IntroVideoUrl = url;

        public void AddDocument(UserDocument document)
        {
            ArgumentNullException.ThrowIfNull(document);
            _documents.Add(document);
        }
    }
}
