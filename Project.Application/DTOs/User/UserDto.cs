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
        string? BloodGroup = null,
        string? EmergencyContact = null)
    {
        public string FullName => $"{FirstName} {LastName}";
    }
}
