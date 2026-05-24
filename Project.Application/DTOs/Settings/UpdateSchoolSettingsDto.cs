namespace Project.Application.DTOs.Settings
{
    public record UpdateSchoolSettingsDto(
        string SchoolName,
        string? Address,
        string? PhoneNumber,
        string? Email);
}
