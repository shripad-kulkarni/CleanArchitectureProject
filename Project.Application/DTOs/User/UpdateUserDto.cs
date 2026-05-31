namespace Project.Application.DTOs.User
{
    public record UpdateUserDto(
        string FirstName,
        string LastName,
        string Phone,
        string Street,
        string City,
        string State,
        string PinCode,
        string? Description = null);
}
