using Project.Application.DTOs.Export;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface IExportService
    {
        byte[] ExportPdf<TRow>(IReadOnlyList<TRow> rows, ExportOptions<TRow> options);
        byte[] ExportExcel<TRow>(IReadOnlyList<TRow> rows, ExportOptions<TRow> options);
        byte[] ExportWord<TRow>(IReadOnlyList<TRow> rows, ExportOptions<TRow> options);
    }
}
