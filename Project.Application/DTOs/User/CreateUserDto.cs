namespace Project.Application.DTOs.User
{
    public record CreateUserDto(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        DateOnly DateOfBirth,
        string Gender,
        string Street,
        string City,
        string State,
        string PinCode,
        string? BloodGroup = null,
        string? EmergencyContact = null,
        string? Description = null);
}
