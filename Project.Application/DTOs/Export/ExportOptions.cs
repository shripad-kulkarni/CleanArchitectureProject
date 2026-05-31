using Project.Application.DTOs.Settings;

namespace Project.Application.DTOs.Export
{
    public sealed class ExportOptions<TRow>
    {
        public required string ReportTitle { get; init; }
        public string? FilterLabel { get; init; }
        public InfoHeaderDto? Header { get; init; }
        public required IReadOnlyList<ExportColumn<TRow>> Columns { get; init; }
        public string TotalLabel { get; init; } = "Total";
    }
}
