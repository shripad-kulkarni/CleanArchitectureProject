namespace Project.Domain.Aggregates.SchoolSettingAggregate
{
    public sealed class SchoolSettings
    {
        private SchoolSettings() { }

        public int Id { get; private set; }
        public string SchoolName { get; private set; } = string.Empty;
        public string? LogoPath { get; private set; }
        public string? Address { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? Email { get; private set; }

        public static SchoolSettings CreateDefault() =>
            new() { Id = 1, SchoolName = "School Management" };

        public void Update(string schoolName, string? address, string? phoneNumber, string? email)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(schoolName);
            SchoolName = schoolName;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        public void SetLogoPath(string? logoPath) => LogoPath = logoPath;
    }
}
