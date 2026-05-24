namespace Project.Application.DTOs.Report
{
    public record StudentReportQueryDto(
        string? ClassName = null,
        string? AcademicYear = null);
}
