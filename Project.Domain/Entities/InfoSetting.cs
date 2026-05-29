using Project.Domain.Primitives;

namespace Project.Domain.Entities
{
    public sealed class InfoSetting : AggregateRoot
    {
        private InfoSetting() { }

        private InfoSetting(int id, string name) : base(id)
        {
            Name = name;
        }

        public string Name { get; private set; } = string.Empty;
        public string? LogoPath { get; private set; }
        public string? Address { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? Email { get; private set; }

        public static InfoSetting CreateDefault() => new(1, "Project Name");

        public void Update(string name, string? address, string? phoneNumber, string? email)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        public void SetLogoPath(string? logoPath) => LogoPath = logoPath;
    }
}
