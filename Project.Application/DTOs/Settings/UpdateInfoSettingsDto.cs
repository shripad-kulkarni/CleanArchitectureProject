namespace Project.Application.DTOs.Settings
{
    public record UpdateInfoSettingsDto(
        string Name,
        string? Address,
        string? PhoneNumber,
        string? Email);
}
