using Project.Application.DTOs.Report;
using Project.Application.DTOs.Settings;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface IUserExportService
    {
        byte[] ExportPdf(IReadOnlyList<UserReportRowDto> rows, string? filterLabel = null, InfoHeaderDto? header = null);
        byte[] ExportExcel(IReadOnlyList<UserReportRowDto> rows, string? filterLabel = null, InfoHeaderDto? header = null);
        byte[] ExportWord(IReadOnlyList<UserReportRowDto> rows, string? filterLabel = null, InfoHeaderDto? header = null);
    }
}
