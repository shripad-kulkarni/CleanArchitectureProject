using Project.Application.DTOs.Report;
using Project.Application.DTOs.Settings;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface IStudentExportService
    {
        byte[] ExportPdf(IReadOnlyList<StudentReportRowDto> rows, string? className, string? academicYear, SchoolHeaderDto? header = null);
        byte[] ExportExcel(IReadOnlyList<StudentReportRowDto> rows, string? className, string? academicYear, SchoolHeaderDto? header = null);
        byte[] ExportWord(IReadOnlyList<StudentReportRowDto> rows, string? className, string? academicYear, SchoolHeaderDto? header = null);
    }
}
