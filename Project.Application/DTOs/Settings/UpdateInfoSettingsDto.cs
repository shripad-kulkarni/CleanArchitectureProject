namespace Project.Application.DTOs.Settings
{
    public record UpdateInfoSettingsDto(
        string SchoolName,
        string? Address,
        string? PhoneNumber,
        string? Email);
}
