namespace Project.Application.DTOs.Report
{
    public record UserReportRowDto(
        int Id,
        string FullName,
        string Gender,
        string Phone,
        string Email,
        string? BloodGroup = null,
        string? EmergencyContact = null);
}
