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
    public sealed class StudentExportService : IStudentExportService
    {
        public byte[] ExportPdf(IReadOnlyList<StudentReportRowDto> rows, string? className, string? academicYear, SchoolHeaderDto? header = null)
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
                                hRow.ConstantItem(60).AlignMiddle()
                                    .Image(logo).FitArea();
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
                                h.Item().AlignCenter().Text("Students Report").Bold().FontSize(12).FontColor(Colors.Grey.Darken2);
                            });
                        });

                        col.Item().Height(8);

                        col.Item().Row(row =>
                        {
                            var filters = new List<string>();
                            if (!string.IsNullOrWhiteSpace(className)) filters.Add($"Class: {className}");
                            if (!string.IsNullOrWhiteSpace(academicYear)) filters.Add($"Year: {academicYear}");
                            row.RelativeItem().Text(filters.Count > 0 ? string.Join("   |   ", filters) : "All Students")
                                .FontSize(9).FontColor(Colors.Grey.Darken1);
                            row.RelativeItem().AlignRight().Text($"Generated: {DateTime.Today:dd/MM/yyyy}")
                                .FontSize(9).FontColor(Colors.Grey.Medium);
                        });

                        col.Item().Height(6);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(28);  // #
                                c.RelativeColumn(2);   // Adm#
                                c.ConstantColumn(50);  // Roll#
                                c.RelativeColumn(3);   // Name
                                c.RelativeColumn(2);   // Class
                                c.RelativeColumn(2);   // Year
                                c.ConstantColumn(50);  // Gender
                                c.RelativeColumn(2);   // Phone
                                c.RelativeColumn(3);   // Parent
                            });

                            void HeaderCell(string text)
                            {
                                table.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                    .Text(text).Bold().FontColor(Colors.White).FontSize(8);
                            }

                            HeaderCell("#");
                            HeaderCell("Adm. No.");
                            HeaderCell("Roll No.");
                            HeaderCell("Student Name");
                            HeaderCell("Class");
                            HeaderCell("Year");
                            HeaderCell("Gender");
                            HeaderCell("Phone");
                            HeaderCell("Parent");

                            for (int i = 0; i < rows.Count; i++)
                            {
                                var r = rows[i];
                                var bg = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                void DataCell(string text)
                                {
                                    table.Cell().Background(bg).Padding(4).Text(text).FontSize(8);
                                }

                                DataCell((i + 1).ToString());
                                DataCell(r.AdmissionNumber);
                                DataCell(r.RollNumber);
                                DataCell(r.FullName);
                                DataCell(r.ClassName);
                                DataCell(r.AcademicYear);
                                DataCell(r.Gender);
                                DataCell(r.Phone);
                                DataCell(r.ParentName ?? "");
                            }
                        });

                        col.Item().Height(8);

                        col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(4)
                            .Text($"Total Students: {rows.Count}")
                            .FontSize(8).SemiBold();
                    });
                });
            }).GeneratePdf();
        }

        public byte[] ExportExcel(IReadOnlyList<StudentReportRowDto> rows, string? className, string? academicYear, SchoolHeaderDto? header = null)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Students Report");

            // Title rows
            ws.Cell(1, 1).Value = $"{header?.SchoolName ?? "SCHOOL MANAGEMENT SYSTEM"} - Students Report";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, 9).Merge();

            var filterText = new List<string>();
            if (!string.IsNullOrWhiteSpace(className)) filterText.Add($"Class: {className}");
            if (!string.IsNullOrWhiteSpace(academicYear)) filterText.Add($"Year: {academicYear}");
            ws.Cell(2, 1).Value = filterText.Count > 0 ? string.Join("   |   ", filterText) : "All Students";
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;
            ws.Range(2, 1, 2, 9).Merge();

            // Headers
            int headerRow = 4;
            string[] headers = ["#", "Admission No.", "Roll No.", "Student Name", "Class", "Academic Year", "Gender", "Phone", "Parent Name"];
            for (int col = 0; col < headers.Length; col++)
            {
                var cell = ws.Cell(headerRow, col + 1);
                cell.Value = headers[col];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e40af");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Data rows
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                int row = headerRow + 1 + i;
                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 2).Value = r.AdmissionNumber;
                ws.Cell(row, 3).Value = r.RollNumber;
                ws.Cell(row, 4).Value = r.FullName;
                ws.Cell(row, 5).Value = r.ClassName;
                ws.Cell(row, 6).Value = r.AcademicYear;
                ws.Cell(row, 7).Value = r.Gender;
                ws.Cell(row, 8).Value = r.Phone;
                ws.Cell(row, 9).Value = r.ParentName ?? "";

                if (i % 2 == 1)
                    ws.Range(row, 1, row, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9fafb");
            }

            int summaryRow = headerRow + rows.Count + 2;
            ws.Cell(summaryRow, 1).Value = $"Total Students: {rows.Count}";
            ws.Cell(summaryRow, 1).Style.Font.Bold = true;

            ws.Columns().AdjustToContents();
            ws.Column(4).Width = Math.Max(ws.Column(4).Width, 25);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public byte[] ExportWord(IReadOnlyList<StudentReportRowDto> rows, string? className, string? academicYear, SchoolHeaderDto? header = null)
        {
            var filterText = new List<string>();
            if (!string.IsNullOrWhiteSpace(className)) filterText.Add($"Class: {className}");
            if (!string.IsNullOrWhiteSpace(academicYear)) filterText.Add($"Year: {academicYear}");
            var filterLine = filterText.Count > 0 ? string.Join("   |   ", filterText) : "All Students";

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
            sb.AppendLine(".summary { margin-top: 12px; font-weight: bold; }");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine($"<h1>{System.Net.WebUtility.HtmlEncode(header?.SchoolName ?? "SCHOOL MANAGEMENT SYSTEM")}</h1>");
            if (!string.IsNullOrWhiteSpace(header?.Address))
                sb.AppendLine($"<p class='sub'>{System.Net.WebUtility.HtmlEncode(header.Address)}</p>");
            if (!string.IsNullOrWhiteSpace(header?.PhoneNumber))
                sb.AppendLine($"<p class='sub'>Phone: {System.Net.WebUtility.HtmlEncode(header.PhoneNumber)}</p>");
            sb.AppendLine("<h2>Students Report</h2>");
            sb.AppendLine($"<div class='filter'>{filterLine} &nbsp;&nbsp; Generated: {DateTime.Today:dd/MM/yyyy}</div>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>#</th><th>Adm. No.</th><th>Roll No.</th><th>Student Name</th><th>Class</th><th>Year</th><th>Gender</th><th>Phone</th><th>Parent Name</th></tr>");

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                sb.AppendLine($"<tr><td>{i + 1}</td><td>{r.AdmissionNumber}</td><td>{r.RollNumber}</td><td>{r.FullName}</td><td>{r.ClassName}</td><td>{r.AcademicYear}</td><td>{r.Gender}</td><td>{r.Phone}</td><td>{r.ParentName ?? ""}</td></tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine($"<div class='summary'>Total Students: {rows.Count}</div>");
            sb.AppendLine("</body></html>");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
