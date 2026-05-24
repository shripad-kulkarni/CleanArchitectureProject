using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.DTOs.Expense;
using Project.Application.DTOs.Settings;
using System.Text;

namespace Project.Infrastructure.Services
{
    public sealed class ExpenseExportService : IExpenseExportService
    {
        public byte[] ExportPdf(IReadOnlyList<ExpenseDto> rows, string? category, DateOnly? fromDate, DateOnly? toDate, SchoolHeaderDto? header = null)
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
                        // School header
                        col.Item().BorderBottom(2).BorderColor(Colors.Blue.Darken2).PaddingBottom(6).Row(hRow =>
                        {
                            if (header?.LogoBytes is { Length: > 0 } logo)
                            {
                                hRow.ConstantItem(60).AlignMiddle().Image(logo).FitArea();
                                hRow.ConstantItem(10);
                            }

                            hRow.RelativeItem().Column(h =>
                            {
                                h.Item().AlignCenter().Text(header?.SchoolName ?? "SCHOOL MANAGEMENT SYSTEM")
                                    .Bold().FontSize(16).FontColor(Colors.Blue.Darken2);
                                if (!string.IsNullOrWhiteSpace(header?.Address))
                                    h.Item().AlignCenter().Text(header.Address).FontSize(9).FontColor(Colors.Grey.Darken1);
                                if (!string.IsNullOrWhiteSpace(header?.PhoneNumber))
                                    h.Item().AlignCenter().Text($"Phone: {header.PhoneNumber}").FontSize(9).FontColor(Colors.Grey.Darken1);
                                h.Item().AlignCenter().Text("Expenses Report").Bold().FontSize(12).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        col.Item().Height(8);

                        col.Item().Row(row =>
                        {
                            var filters = new List<string>();
                            if (!string.IsNullOrWhiteSpace(category)) filters.Add($"Category: {category}");
                            if (fromDate.HasValue) filters.Add($"From: {fromDate.Value:dd/MM/yyyy}");
                            if (toDate.HasValue) filters.Add($"To: {toDate.Value:dd/MM/yyyy}");
                            row.RelativeItem().Text(filters.Count > 0 ? string.Join("   |   ", filters) : "All Expenses")
                                .FontSize(9).FontColor(Colors.Grey.Darken1);
                            row.RelativeItem().AlignRight().Text($"Generated: {DateTime.Today:dd/MM/yyyy}")
                                .FontSize(9).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().Height(6);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(28);
                                c.RelativeColumn(4);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                                c.RelativeColumn(4);
                            });

                            void HeaderCell(string text)
                            {
                                table.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                    .Text(text).Bold().FontColor(Colors.White).FontSize(8);
                            }

                            HeaderCell("#");
                            HeaderCell("Title");
                            HeaderCell("Category");
                            HeaderCell("Amount (₹)");
                            HeaderCell("Date");
                            HeaderCell("Description");

                            for (int i = 0; i < rows.Count; i++)
                            {
                                var r = rows[i];
                                var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                void DataCell(string text, bool rightAlign = false)
                                {
                                    var cell = table.Cell().Background(bg).Padding(4);
                                    if (rightAlign) cell.AlignRight().Text(text).FontSize(8);
                                    else cell.Text(text).FontSize(8);
                                }

                                DataCell((i + 1).ToString());
                                DataCell(r.Title);
                                DataCell(r.Category);
                                DataCell(r.Amount.ToString("N2"), rightAlign: true);
                                DataCell(r.ExpenseDate.ToString("dd/MM/yyyy"));
                                DataCell(r.Description ?? "");
                            }
                        });

                        col.Item().Height(8);

                        var total = rows.Sum(r => r.Amount);
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Total Records: {rows.Count}").FontSize(8).SemiBold();
                            row.RelativeItem().AlignRight()
                                .Text($"Total Amount: ₹{total:N2}").FontSize(8).SemiBold();
                        });
                    });
                });
            }).GeneratePdf();
        }

        public byte[] ExportExcel(IReadOnlyList<ExpenseDto> rows, string? category, DateOnly? fromDate, DateOnly? toDate, SchoolHeaderDto? header = null)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Expenses Report");

            ws.Cell(1, 1).Value = $"{header?.SchoolName ?? "SCHOOL MANAGEMENT SYSTEM"} - Expenses Report";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, 6).Merge();

            var filterParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(category)) filterParts.Add($"Category: {category}");
            if (fromDate.HasValue) filterParts.Add($"From: {fromDate.Value:dd/MM/yyyy}");
            if (toDate.HasValue) filterParts.Add($"To: {toDate.Value:dd/MM/yyyy}");
            ws.Cell(2, 1).Value = filterParts.Count > 0 ? string.Join("   |   ", filterParts) : "All Expenses";
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;
            ws.Range(2, 1, 2, 6).Merge();

            int headerRow = 4;
            string[] headers = ["#", "Title", "Category", "Amount (₹)", "Date", "Description"];
            for (int col = 0; col < headers.Length; col++)
            {
                var cell = ws.Cell(headerRow, col + 1);
                cell.Value = headers[col];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e40af");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                int row = headerRow + 1 + i;
                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 2).Value = r.Title;
                ws.Cell(row, 3).Value = r.Category;
                ws.Cell(row, 4).Value = r.Amount;
                ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(row, 5).Value = r.ExpenseDate.ToString("dd/MM/yyyy");
                ws.Cell(row, 6).Value = r.Description ?? "";

                if (i % 2 == 1)
                    ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9fafb");
            }

            int summaryRow = headerRow + rows.Count + 2;
            ws.Cell(summaryRow, 1).Value = $"Total Records: {rows.Count}";
            ws.Cell(summaryRow, 1).Style.Font.Bold = true;
            ws.Cell(summaryRow, 4).Value = rows.Sum(r => r.Amount);
            ws.Cell(summaryRow, 4).Style.Font.Bold = true;
            ws.Cell(summaryRow, 4).Style.NumberFormat.Format = "#,##0.00";

            ws.Columns().AdjustToContents();
            ws.Column(2).Width = Math.Max(ws.Column(2).Width, 30);
            ws.Column(6).Width = Math.Max(ws.Column(6).Width, 30);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExportWord(IReadOnlyList<ExpenseDto> rows, string? category, DateOnly? fromDate, DateOnly? toDate, SchoolHeaderDto? header = null)
        {
            var filterParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(category)) filterParts.Add($"Category: {category}");
            if (fromDate.HasValue) filterParts.Add($"From: {fromDate.Value:dd/MM/yyyy}");
            if (toDate.HasValue) filterParts.Add($"To: {toDate.Value:dd/MM/yyyy}");
            var filterLine = filterParts.Count > 0 ? string.Join("   |   ", filterParts) : "All Expenses";
            var total = rows.Sum(r => r.Amount);

            var sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset='utf-8'>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; font-size: 11pt; }");
            sb.AppendLine("h1 { color: #1e40af; font-size: 16pt; text-align: center; margin-bottom: 4px; }");
            sb.AppendLine("h2 { color: #374151; font-size: 13pt; text-align: center; margin-top: 0; }");
            sb.AppendLine(".sub { color: #6b7280; font-size: 9pt; text-align: center; margin: 0; }");
            sb.AppendLine(".filter { color: #6b7280; font-size: 10pt; margin-bottom: 12px; }");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; font-size: 9pt; }");
            sb.AppendLine("th { background-color: #1e40af; color: white; padding: 6px 8px; text-align: left; }");
            sb.AppendLine("td { border: 1px solid #e5e7eb; padding: 5px 8px; }");
            sb.AppendLine("tr:nth-child(even) td { background-color: #f9fafb; }");
            sb.AppendLine(".summary { margin-top: 12px; font-weight: bold; display: flex; justify-content: space-between; }");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine($"<h1>{System.Net.WebUtility.HtmlEncode(header?.SchoolName ?? "SCHOOL MANAGEMENT SYSTEM")}</h1>");
            if (!string.IsNullOrWhiteSpace(header?.Address))
                sb.AppendLine($"<p class='sub'>{System.Net.WebUtility.HtmlEncode(header.Address)}</p>");
            if (!string.IsNullOrWhiteSpace(header?.PhoneNumber))
                sb.AppendLine($"<p class='sub'>Phone: {System.Net.WebUtility.HtmlEncode(header.PhoneNumber)}</p>");
            sb.AppendLine("<h2>Expenses Report</h2>");
            sb.AppendLine($"<div class='filter'>{filterLine} &nbsp;&nbsp; Generated: {DateTime.Today:dd/MM/yyyy}</div>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>#</th><th>Title</th><th>Category</th><th>Amount (₹)</th><th>Date</th><th>Description</th></tr>");

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                sb.AppendLine($"<tr><td>{i + 1}</td><td>{r.Title}</td><td>{r.Category}</td><td style='text-align:right'>{r.Amount:N2}</td><td>{r.ExpenseDate:dd/MM/yyyy}</td><td>{r.Description ?? ""}</td></tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine($"<div class='summary'><span>Total Records: {rows.Count}</span><span>Total Amount: ₹{total:N2}</span></div>");
            sb.AppendLine("</body></html>");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
