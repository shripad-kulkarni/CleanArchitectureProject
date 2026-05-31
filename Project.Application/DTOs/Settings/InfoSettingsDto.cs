namespace Project.Application.DTOs.Settings
{
    public record InfoSettingsDto(
        string Name,
        string? LogoPath,
        string? Address,
        string? PhoneNumber,
        string? Email);
}
