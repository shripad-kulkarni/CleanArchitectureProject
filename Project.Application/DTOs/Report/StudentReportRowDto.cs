namespace Project.Application.DTOs.Report
{
    public record StudentReportRowDto(
        int Id,
        string AdmissionNumber,
        string RollNumber,
        string FullName,
        string ClassName,
        string AcademicYear,
        string Gender,
        string Phone,
        string Email,
        string? ParentName,
        string? ParentPhone);
}
