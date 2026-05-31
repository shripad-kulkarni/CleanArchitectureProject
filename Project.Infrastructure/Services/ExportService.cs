using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.DTOs.Export;
using System.Net;
using System.Text;

namespace Project.Infrastructure.Services
{
    public sealed class ExportService : IExportService
    {
        public byte[] ExportPdf<TRow>(IReadOnlyList<TRow> rows, ExportOptions<TRow> options)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9).FontColor(Colors.Grey.Darken3));

                    page.Content().Column(col =>
                    {
                        col.Item().BorderBottom(2).BorderColor(Colors.Blue.Darken2).PaddingBottom(6).Row(hRow =>
                        {
                            if (options.Header?.LogoBytes is { Length: > 0 } logo)
                            {
                                hRow.ConstantItem(60).AlignMiddle().Image(logo).FitArea();
                                hRow.ConstantItem(10);
                            }

                            hRow.RelativeItem().Column(h =>
                            {
                                h.Item().AlignCenter().Text(options.Header?.Name ?? "MANAGEMENT SYSTEM")
                                    .Bold().FontSize(16).FontColor(Colors.Blue.Darken2);
                                if (!string.IsNullOrWhiteSpace(options.Header?.Address))
                                    h.Item().AlignCenter().Text(options.Header.Address).FontSize(9).FontColor(Colors.Grey.Darken1);
                                if (!string.IsNullOrWhiteSpace(options.Header?.PhoneNumber))
                                    h.Item().AlignCenter().Text($"Phone: {options.Header.PhoneNumber}").FontSize(9).FontColor(Colors.Grey.Darken1);
                                h.Item().AlignCenter().Text(options.ReportTitle).Bold().FontSize(12).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        col.Item().Height(8);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text(options.FilterLabel ?? "All Records").FontSize(9).FontColor(Colors.Grey.Darken1);
                            row.RelativeItem().AlignRight().Text($"Generated: {DateTime.Today:dd/MM/yyyy}")
                                .FontSize(9).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().Height(6);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                foreach (var column in options.Columns)
                                {
                                    if (column.ConstantWidth.HasValue)
                                        c.ConstantColumn(column.ConstantWidth.Value);
                                    else
                                        c.RelativeColumn(column.RelativeWidth);
                                }
                            });

                            void HeaderCell(string text) =>
                                table.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                    .Text(text).Bold().FontColor(Colors.White).FontSize(8);

                            foreach (var column in options.Columns)
                                HeaderCell(column.Header);

                            for (int i = 0; i < rows.Count; i++)
                            {
                                var r = rows[i];
                                var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                void DataCell(string text) =>
                                    table.Cell().Background(bg).Padding(4).Text(text).FontSize(8);

                                foreach (var column in options.Columns)
                                    DataCell(column.ValueSelector(r, i));
                            }
                        });

                        col.Item().Height(8);
                        col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(4)
                            .Text($"{options.TotalLabel}: {rows.Count}").FontSize(8).SemiBold();
                    });
                });
            }).GeneratePdf();
        }

        public byte[] ExportExcel<TRow>(IReadOnlyList<TRow> rows, ExportOptions<TRow> options)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(options.ReportTitle);
            int colCount = options.Columns.Count;

            ws.Cell(1, 1).Value = $"{options.Header?.Name ?? "MANAGEMENT SYSTEM"} - {options.ReportTitle}";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, colCount).Merge();

            ws.Cell(2, 1).Value = options.FilterLabel ?? "All Records";
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;
            ws.Range(2, 1, 2, colCount).Merge();

            const int headerRow = 4;
            for (int c = 0; c < options.Columns.Count; c++)
            {
                var cell = ws.Cell(headerRow, c + 1);
                cell.Value = options.Columns[c].Header;
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e40af");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            for (int i = 0; i < rows.Count; i++)
            {
                int row = headerRow + 1 + i;
                for (int c = 0; c < options.Columns.Count; c++)
                    ws.Cell(row, c + 1).Value = options.Columns[c].ValueSelector(rows[i], i);

                if (i % 2 == 1)
                    ws.Range(row, 1, row, colCount).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9fafb");
            }

            ws.Cell(headerRow + rows.Count + 2, 1).Value = $"{options.TotalLabel}: {rows.Count}";
            ws.Cell(headerRow + rows.Count + 2, 1).Style.Font.Bold = true;

            ws.Columns().AdjustToContents();
            foreach (var wsCol in ws.Columns())
                if (wsCol.Width < 12) wsCol.Width = 12;

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExportWord<TRow>(IReadOnlyList<TRow> rows, ExportOptions<TRow> options)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset='utf-8'><style>");
            sb.AppendLine("body{font-family:Arial,sans-serif;font-size:11pt}");
            sb.AppendLine("h1{color:#1e40af;font-size:16pt;text-align:center;margin-bottom:4px}");
            sb.AppendLine("h2{color:#374151;font-size:13pt;text-align:center;margin-top:0}");
            sb.AppendLine(".sub{color:#6b7280;font-size:9pt;text-align:center;margin:0}");
            sb.AppendLine("table{border-collapse:collapse;width:100%;font-size:9pt}");
            sb.AppendLine("th{background-color:#1e40af;color:white;padding:6px 8px;text-align:left}");
            sb.AppendLine("td{border:1px solid #e5e7eb;padding:5px 8px}");
            sb.AppendLine("tr:nth-child(even) td{background-color:#f9fafb}");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine($"<h1>{WebUtility.HtmlEncode(options.Header?.Name ?? "MANAGEMENT SYSTEM")}</h1>");
            if (!string.IsNullOrWhiteSpace(options.Header?.Address))
                sb.AppendLine($"<p class='sub'>{WebUtility.HtmlEncode(options.Header.Address)}</p>");
            if (!string.IsNullOrWhiteSpace(options.Header?.PhoneNumber))
                sb.AppendLine($"<p class='sub'>Phone: {WebUtility.HtmlEncode(options.Header.PhoneNumber)}</p>");
            sb.AppendLine($"<h2>{WebUtility.HtmlEncode(options.ReportTitle)}</h2>");
            sb.AppendLine($"<div>{WebUtility.HtmlEncode(options.FilterLabel ?? "All Records")} &nbsp; Generated: {DateTime.Today:dd/MM/yyyy}</div>");

            sb.Append("<table><tr>");
            foreach (var column in options.Columns)
                sb.Append($"<th>{WebUtility.HtmlEncode(column.Header)}</th>");
            sb.AppendLine("</tr>");

            for (int i = 0; i < rows.Count; i++)
            {
                sb.Append("<tr>");
                foreach (var column in options.Columns)
                    sb.Append($"<td>{WebUtility.HtmlEncode(column.ValueSelector(rows[i], i))}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine($"</table><div style='margin-top:12px;font-weight:bold'>{options.TotalLabel}: {rows.Count}</div></body></html>");
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
