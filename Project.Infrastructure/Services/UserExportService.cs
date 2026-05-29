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
    public sealed class UserExportService : IUserExportService
    {
        public byte[] ExportPdf(IReadOnlyList<UserReportRowDto> rows, string? filterLabel = null, InfoHeaderDto? header = null)
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
                            if (header?.LogoBytes is { Length: > 0 } logo)
                            {
                                hRow.ConstantItem(60).AlignMiddle().Image(logo).FitArea();
                                hRow.ConstantItem(10);
                            }

                            hRow.RelativeItem().Column(h =>
                            {
                                h.Item().AlignCenter().Text(header?.SchoolName ?? "MANAGEMENT SYSTEM")
                                    .Bold().FontSize(16).FontColor(Colors.Blue.Darken2);
                                if (!string.IsNullOrWhiteSpace(header?.Address))
                                    h.Item().AlignCenter().Text(header.Address).FontSize(9).FontColor(Colors.Grey.Darken1);
                                if (!string.IsNullOrWhiteSpace(header?.PhoneNumber))
                                    h.Item().AlignCenter().Text($"Phone: {header.PhoneNumber}").FontSize(9).FontColor(Colors.Grey.Darken1);
                                h.Item().AlignCenter().Text("Users Report").Bold().FontSize(12).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        col.Item().Height(8);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text(filterLabel ?? "All Users").FontSize(9).FontColor(Colors.Grey.Darken1);
                            row.RelativeItem().AlignRight().Text($"Generated: {DateTime.Today:dd/MM/yyyy}")
                                .FontSize(9).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().Height(6);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(28);
                                c.RelativeColumn(3);
                                c.ConstantColumn(50);
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });

                            void HeaderCell(string text) =>
                                table.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                    .Text(text).Bold().FontColor(Colors.White).FontSize(8);

                            HeaderCell("#"); HeaderCell("Name"); HeaderCell("Gender");
                            HeaderCell("Email"); HeaderCell("Phone"); HeaderCell("Blood Group");

                            for (int i = 0; i < rows.Count; i++)
                            {
                                var r = rows[i];
                                var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                void DataCell(string text) =>
                                    table.Cell().Background(bg).Padding(4).Text(text).FontSize(8);

                                DataCell((i + 1).ToString()); DataCell(r.FullName); DataCell(r.Gender);
                                DataCell(r.Email); DataCell(r.Phone); DataCell(r.BloodGroup ?? "");
                            }
                        });

                        col.Item().Height(8);
                        col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(4)
                            .Text($"Total Users: {rows.Count}").FontSize(8).SemiBold();
                    });
                });
            }).GeneratePdf();
        }

        public byte[] ExportExcel(IReadOnlyList<UserReportRowDto> rows, string? filterLabel = null, InfoHeaderDto? header = null)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Users Report");

            ws.Cell(1, 1).Value = $"{header?.SchoolName ?? "MANAGEMENT SYSTEM"} - Users Report";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, 6).Merge();

            ws.Cell(2, 1).Value = filterLabel ?? "All Users";
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;
            ws.Range(2, 1, 2, 6).Merge();

            int headerRow = 4;
            string[] headers = ["#", "Name", "Gender", "Email", "Phone", "Blood Group"];
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
                ws.Cell(row, 2).Value = r.FullName;
                ws.Cell(row, 3).Value = r.Gender;
                ws.Cell(row, 4).Value = r.Email;
                ws.Cell(row, 5).Value = r.Phone;
                ws.Cell(row, 6).Value = r.BloodGroup ?? "";

                if (i % 2 == 1)
                    ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9fafb");
            }

            ws.Cell(headerRow + rows.Count + 2, 1).Value = $"Total Users: {rows.Count}";
            ws.Cell(headerRow + rows.Count + 2, 1).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();
            ws.Column(2).Width = Math.Max(ws.Column(2).Width, 25);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExportWord(IReadOnlyList<UserReportRowDto> rows, string? filterLabel = null, InfoHeaderDto? header = null)
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
            sb.AppendLine($"<h1>{System.Net.WebUtility.HtmlEncode(header?.SchoolName ?? "MANAGEMENT SYSTEM")}</h1>");
            if (!string.IsNullOrWhiteSpace(header?.Address))
                sb.AppendLine($"<p class='sub'>{System.Net.WebUtility.HtmlEncode(header.Address)}</p>");
            if (!string.IsNullOrWhiteSpace(header?.PhoneNumber))
                sb.AppendLine($"<p class='sub'>Phone: {System.Net.WebUtility.HtmlEncode(header.PhoneNumber)}</p>");
            sb.AppendLine("<h2>Users Report</h2>");
            sb.AppendLine($"<div>{filterLabel ?? "All Users"} &nbsp; Generated: {DateTime.Today:dd/MM/yyyy}</div>");
            sb.AppendLine("<table><tr><th>#</th><th>Name</th><th>Gender</th><th>Email</th><th>Phone</th><th>Blood Group</th></tr>");

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                sb.AppendLine($"<tr><td>{i + 1}</td><td>{r.FullName}</td><td>{r.Gender}</td><td>{r.Email}</td><td>{r.Phone}</td><td>{r.BloodGroup ?? ""}</td></tr>");
            }

            sb.AppendLine($"</table><div style='margin-top:12px;font-weight:bold'>Total Users: {rows.Count}</div></body></html>");
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
