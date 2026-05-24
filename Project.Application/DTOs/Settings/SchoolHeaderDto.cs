namespace Project.Application.DTOs.Settings
{
    // Passed to export services so they can render the school header in generated PDFs.
    public record SchoolHeaderDto(
        string SchoolName,
        string? Address,
        string? PhoneNumber,
        byte[]? LogoBytes);
}
