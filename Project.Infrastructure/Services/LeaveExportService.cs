using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.DTOs.Report;
using Project.Application.DTOs.Settings;
using System.Text;

namespace Project.Infrastructure.Services
{
    public sealed class LeaveExportService : ILeaveExportService
    {
        public byte[] ExportPdf(IReadOnlyList<LeaveReportRowDto> rows, LeaveReportQueryDto query, SchoolHeaderDto? header = null)
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
                                h.Item().AlignCenter().Text("Leave Report").Bold().FontSize(12).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        col.Item().Height(8);

                        col.Item().Row(row =>
                        {
                            var filters = BuildFilterLine(query);
                            row.RelativeItem().Text(filters).FontSize(9).FontColor(Colors.Grey.Darken1);
                            row.RelativeItem().AlignRight().Text($"Generated: {DateTime.Today:dd/MM/yyyy}")
                                .FontSize(9).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().Height(6);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(28);   // #
                                c.RelativeColumn(3);    // Staff Name
                                c.RelativeColumn(2);    // Leave Type
                                c.RelativeColumn(2);    // From
                                c.RelativeColumn(2);    // To
                                c.ConstantColumn(40);   // Days
                                c.RelativeColumn(2);    // Status
                                c.RelativeColumn(4);    // Reason
                            });

                            void HeaderCell(string text)
                            {
                                table.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                    .Text(text).Bold().FontColor(Colors.White).FontSize(8);
                            }

                            HeaderCell("#");
                            HeaderCell("Staff Name");
                            HeaderCell("Leave Type");
                            HeaderCell("From");
                            HeaderCell("To");
                            HeaderCell("Days");
                            HeaderCell("Status");
                            HeaderCell("Reason");

                            for (int i = 0; i < rows.Count; i++)
                            {
                                var r = rows[i];
                                var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                void DataCell(string text)
                                {
                                    table.Cell().Background(bg).Padding(4).Text(text).FontSize(8);
                                }

                                DataCell((i + 1).ToString());
                                DataCell(r.StaffName);
                                DataCell(r.LeaveType);
                                DataCell(r.StartDate.ToString("dd/MM/yyyy"));
                                DataCell(r.EndDate.ToString("dd/MM/yyyy"));
                                DataCell(r.TotalDays.ToString());
                                DataCell(r.Status);
                                DataCell(r.Reason);
                            }
                        });

                        col.Item().Height(8);

                        int totalDays = rows.Sum(r => r.TotalDays);
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Total Records: {rows.Count}").FontSize(8).SemiBold();
                            row.RelativeItem().AlignRight()
                                .Text($"Total Days: {totalDays}").FontSize(8).SemiBold();
                        });
                    });
                });
            }).GeneratePdf();
        }

        public byte[] ExportExcel(IReadOnlyList<LeaveReportRowDto> rows, LeaveReportQueryDto query, SchoolHeaderDto? header = null)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Leave Report");

            ws.Cell(1, 1).Value = $"{header?.SchoolName ?? "SCHOOL MANAGEMENT SYSTEM"} - Leave Report";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, 8).Merge();

            ws.Cell(2, 1).Value = BuildFilterLine(query);
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;
            ws.Range(2, 1, 2, 8).Merge();

            int headerRow = 4;
            string[] headers = ["#", "Staff Name", "Leave Type", "From", "To", "Days", "Status", "Reason"];
            for (int col = 0; col < headers.Length; col++)
            {
                var cell = ws.Cell(headerRow, col + 1);
                cell.Value = headers[col];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e40af");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            var statusColors = new Dictionary<string, string>
            {
                ["Approved"]  = "#dcfce7",
                ["Pending"]   = "#fef9c3",
                ["Rejected"]  = "#fee2e2",
                ["Cancelled"] = "#f3f4f6",
            };

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                int row = headerRow + 1 + i;
                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 2).Value = r.StaffName;
                ws.Cell(row, 3).Value = r.LeaveType;
                ws.Cell(row, 4).Value = r.StartDate.ToString("dd/MM/yyyy");
                ws.Cell(row, 5).Value = r.EndDate.ToString("dd/MM/yyyy");
                ws.Cell(row, 6).Value = r.TotalDays;
                ws.Cell(row, 7).Value = r.Status;
                ws.Cell(row, 8).Value = r.Reason;

                if (statusColors.TryGetValue(r.Status, out var color))
                    ws.Cell(row, 7).Style.Fill.BackgroundColor = XLColor.FromHtml(color);
                else if (i % 2 == 1)
                    ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9fafb");
            }

            int summaryRow = headerRow + rows.Count + 2;
            ws.Cell(summaryRow, 1).Value = $"Total Records: {rows.Count}";
            ws.Cell(summaryRow, 1).Style.Font.Bold = true;
            ws.Cell(summaryRow, 6).Value = rows.Sum(r => r.TotalDays);
            ws.Cell(summaryRow, 6).Style.Font.Bold = true;

            ws.Columns().AdjustToContents();
            ws.Column(2).Width = Math.Max(ws.Column(2).Width, 22);
            ws.Column(8).Width = Math.Max(ws.Column(8).Width, 35);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExportWord(IReadOnlyList<LeaveReportRowDto> rows, LeaveReportQueryDto query, SchoolHeaderDto? header = null)
        {
            var filterLine = BuildFilterLine(query);
            int totalDays = rows.Sum(r => r.TotalDays);

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
            sb.AppendLine(".status-approved { background-color: #dcfce7; }");
            sb.AppendLine(".status-pending { background-color: #fef9c3; }");
            sb.AppendLine(".status-rejected { background-color: #fee2e2; }");
            sb.AppendLine(".summary { margin-top: 12px; font-weight: bold; display: flex; justify-content: space-between; }");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine($"<h1>{System.Net.WebUtility.HtmlEncode(header?.SchoolName ?? "SCHOOL MANAGEMENT SYSTEM")}</h1>");
            if (!string.IsNullOrWhiteSpace(header?.Address))
                sb.AppendLine($"<p class='sub'>{System.Net.WebUtility.HtmlEncode(header.Address)}</p>");
            if (!string.IsNullOrWhiteSpace(header?.PhoneNumber))
                sb.AppendLine($"<p class='sub'>Phone: {System.Net.WebUtility.HtmlEncode(header.PhoneNumber)}</p>");
            sb.AppendLine("<h2>Leave Report</h2>");
            sb.AppendLine($"<div class='filter'>{filterLine} &nbsp;&nbsp; Generated: {DateTime.Today:dd/MM/yyyy}</div>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>#</th><th>Staff Name</th><th>Leave Type</th><th>From</th><th>To</th><th>Days</th><th>Status</th><th>Reason</th></tr>");

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                var statusClass = r.Status.ToLower() switch
                {
                    "approved" => "status-approved",
                    "pending"  => "status-pending",
                    "rejected" => "status-rejected",
                    _          => ""
                };
                sb.AppendLine($"<tr><td>{i + 1}</td><td>{r.StaffName}</td><td>{r.LeaveType}</td><td>{r.StartDate:dd/MM/yyyy}</td><td>{r.EndDate:dd/MM/yyyy}</td><td>{r.TotalDays}</td><td class='{statusClass}'>{r.Status}</td><td>{System.Net.WebUtility.HtmlEncode(r.Reason)}</td></tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine($"<div class='summary'><span>Total Records: {rows.Count}</span><span>Total Days: {totalDays}</span></div>");
            sb.AppendLine("</body></html>");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static string BuildFilterLine(LeaveReportQueryDto query)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(query.Status)) parts.Add($"Status: {query.Status}");
            if (!string.IsNullOrWhiteSpace(query.LeaveType)) parts.Add($"Type: {query.LeaveType}");
            if (query.FromDate.HasValue) parts.Add($"From: {query.FromDate.Value:dd/MM/yyyy}");
            if (query.ToDate.HasValue) parts.Add($"To: {query.ToDate.Value:dd/MM/yyyy}");
            return parts.Count > 0 ? string.Join("   |   ", parts) : "All Leaves";
        }
    }
}
