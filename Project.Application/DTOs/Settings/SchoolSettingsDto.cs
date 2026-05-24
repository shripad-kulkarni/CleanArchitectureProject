namespace Project.Application.DTOs.Settings
{
    public record SchoolSettingsDto(
        string SchoolName,
        string? LogoPath,
        string? Address,
        string? PhoneNumber,
        string? Email);
}
