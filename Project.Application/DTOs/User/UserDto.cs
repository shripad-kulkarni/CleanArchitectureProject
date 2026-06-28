namespace Project.Application.DTOs.User
{
    public record UserDto(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        DateOnly DateOfBirth,
        string Gender,
        string Street,
        string City,
        string State,
        string PinCode,
        bool IsActive = true,
        string? BloodGroup = null,
        string? EmergencyContact = null,
        string? Description = null,
        string? ProfilePhotoUrl = null,
        string? IntroVideoUrl = null)
    {
        public string FullName => $"{FirstName} {LastName}";
    }
}
