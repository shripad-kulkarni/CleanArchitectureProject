using Project.Application.DTOs.Report;
using Project.Application.DTOs.Settings;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface ILeaveExportService
    {
        byte[] ExportPdf(IReadOnlyList<LeaveReportRowDto> rows, LeaveReportQueryDto query, SchoolHeaderDto? header = null);
        byte[] ExportExcel(IReadOnlyList<LeaveReportRowDto> rows, LeaveReportQueryDto query, SchoolHeaderDto? header = null);
        byte[] ExportWord(IReadOnlyList<LeaveReportRowDto> rows, LeaveReportQueryDto query, SchoolHeaderDto? header = null);
    }
}
