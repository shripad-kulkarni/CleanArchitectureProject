namespace Project.Application.DTOs.Settings
{
    public record InfoSettingsDto(
        string SchoolName,
        string? LogoPath,
        string? Address,
        string? PhoneNumber,
        string? Email);
}
