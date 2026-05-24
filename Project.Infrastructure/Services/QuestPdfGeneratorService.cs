using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.DTOs.Student;

namespace Project.Infrastructure.Services
{
    public sealed class QuestPdfGeneratorService : IPdfGeneratorService
    {
        public byte[] GenerateStudentProfileReport(StudentDto student)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(11).FontColor(Colors.Grey.Darken3));

                    page.Content().Column(col =>
                    {
                        // Header
                        col.Item().BorderBottom(2).BorderColor(Colors.Blue.Darken2).PaddingBottom(8).Column(h =>
                        {
                            h.Item().AlignCenter().Text("SCHOOL MANAGEMENT SYSTEM")
                                .Bold().FontSize(18).FontColor(Colors.Blue.Darken2);
                            h.Item().AlignCenter().Text("Student Profile Report")
                                .Bold().FontSize(14).FontColor(Colors.Grey.Darken2);
                        });

                        col.Item().Height(16);

                        col.Item().AlignRight().Text($"Generated: {DateTime.Today:dd/MM/yyyy}")
                            .FontSize(9).FontColor(Colors.Grey.Medium);

                        col.Item().Height(12);

                        // Academic Information section
                        col.Item().Background(Colors.Blue.Darken2).Padding(6)
                            .Text("Academic Information").Bold().FontSize(11).FontColor(Colors.White);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                            });

                            void Cell(string label, string value)
                            {
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text(label).SemiBold().FontSize(10);
                                table.Cell().Padding(6).Text(value).FontSize(10);
                            }

                            Cell("Admission No.", student.AdmissionNumber);
                            Cell("Roll Number", student.RollNumber);
                            Cell("Class", student.ClassName);
                            Cell("Academic Year", student.AcademicYear);
                            Cell("Admission Date", student.AdmissionDate.ToString("dd/MM/yyyy"));
                            Cell("", "");
                        });

                        col.Item().Height(12);

                        // Personal Information section
                        col.Item().Background(Colors.Blue.Darken2).Padding(6)
                            .Text("Personal Information").Bold().FontSize(11).FontColor(Colors.White);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                            });

                            void Cell(string label, string value)
                            {
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text(label).SemiBold().FontSize(10);
                                table.Cell().Padding(6).Text(value).FontSize(10);
                            }

                            Cell("Full Name", $"{student.FirstName} {student.LastName}");
                            Cell("Gender", student.Gender);
                            Cell("Date of Birth", student.DateOfBirth.ToString("dd/MM/yyyy"));
                            Cell("Blood Group", string.IsNullOrWhiteSpace(student.BloodGroup) ? "—" : student.BloodGroup);
                        });

                        col.Item().Height(12);

                        // Contact Information section
                        col.Item().Background(Colors.Blue.Darken2).Padding(6)
                            .Text("Contact Information").Bold().FontSize(11).FontColor(Colors.White);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                            });

                            void Cell(string label, string value)
                            {
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text(label).SemiBold().FontSize(10);
                                table.Cell().Padding(6).Text(value).FontSize(10);
                            }

                            Cell("Email", student.Email);
                            Cell("Phone", student.Phone);
                            Cell("City", student.City);
                            Cell("State", student.State);
                            Cell("Street", student.Street);
                            Cell("Pin Code", student.PinCode);
                        });

                        col.Item().Height(12);

                        // Parent / Guardian section
                        col.Item().Background(Colors.Blue.Darken2).Padding(6)
                            .Text("Parent / Guardian").Bold().FontSize(11).FontColor(Colors.White);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                            });

                            void Cell(string label, string value)
                            {
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text(label).SemiBold().FontSize(10);
                                table.Cell().Padding(6).Text(value).FontSize(10);
                            }

                            Cell("Parent Name", string.IsNullOrWhiteSpace(student.ParentName) ? "—" : student.ParentName);
                            Cell("Parent Phone", string.IsNullOrWhiteSpace(student.ParentPhone) ? "—" : student.ParentPhone);
                            Cell("Parent Email", string.IsNullOrWhiteSpace(student.ParentEmail) ? "—" : student.ParentEmail);
                            Cell("Emergency Contact", string.IsNullOrWhiteSpace(student.EmergencyContact) ? "—" : student.EmergencyContact);
                        });

                        col.Item().Height(40);

                        col.Item().AlignRight().Column(sig =>
                        {
                            sig.Item().Text("___________________________").FontColor(Colors.Grey.Darken1);
                            sig.Item().AlignRight().Text("Principal / Head of Institution").SemiBold();
                            sig.Item().AlignRight().Text("(Authorised Signatory)").FontColor(Colors.Grey.Medium);
                        });

                        col.Item().Height(20);

                        col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8)
                            .Text("This report is system-generated and is valid for official reference purposes.")
                            .FontSize(9).Italic().FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }


        public byte[] GenerateBonafideCertificate(StudentDto student)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(11).FontColor(Colors.Grey.Darken3));

                    page.Content().Column(col =>
                    {
                        // Header
                        col.Item().BorderBottom(2).BorderColor(Colors.Blue.Darken2).PaddingBottom(8).Column(h =>
                        {
                            h.Item().AlignCenter().Text("SCHOOL MANAGEMENT SYSTEM")
                                .Bold().FontSize(18).FontColor(Colors.Blue.Darken2);
                            h.Item().AlignCenter().Text("Bonafide Certificate")
                                .Bold().FontSize(14).FontColor(Colors.Grey.Darken2);
                        });

                        col.Item().Height(24);

                        // Certificate number + date row
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Certificate No: BON/{student.AdmissionNumber}/{DateTime.Today:yyyy}");
                            row.RelativeItem().AlignRight().Text($"Date: {DateTime.Today:dd/MM/yyyy}");
                        });

                        col.Item().Height(20);

                        // Body text
                        col.Item().Text(text =>
                        {
                            text.Span("This is to certify that ");
                            text.Span($"{student.FirstName} {student.LastName}").Bold();
                            text.Span($", bearing Admission Number ");
                            text.Span(student.AdmissionNumber).Bold();
                            text.Span($", is a bonafide student of this institution.");
                        });

                        col.Item().Height(12);

                        col.Item().Text(text =>
                        {
                            text.Span("The student is presently studying in Class ");
                            text.Span(student.ClassName).Bold();
                            text.Span(" during the Academic Year ");
                            text.Span(student.AcademicYear).Bold();
                            text.Span(".");
                        });

                        col.Item().Height(12);

                        // Details table
                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                            });

                            void Row(string label, string value)
                            {
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text(label).SemiBold();
                                table.Cell().Padding(6).Text(value);
                            }

                            Row("Full Name", $"{student.FirstName} {student.LastName}");
                            Row("Date of Birth", student.DateOfBirth.ToString("dd/MM/yyyy"));
                            Row("Roll Number", student.RollNumber);
                            Row("Class", student.ClassName);
                            Row("Academic Year", student.AcademicYear);
                            if (!string.IsNullOrWhiteSpace(student.BloodGroup))
                                Row("Blood Group", student.BloodGroup);
                        });

                        col.Item().Height(40);

                        // Signature
                        col.Item().AlignRight().Column(sig =>
                        {
                            sig.Item().Text("___________________________").FontColor(Colors.Grey.Darken1);
                            sig.Item().AlignRight().Text("Principal / Head of Institution").SemiBold();
                            sig.Item().AlignRight().Text("(Authorised Signatory)").FontColor(Colors.Grey.Medium);
                        });

                        col.Item().Height(20);

                        // Footer note
                        col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8)
                            .Text("This certificate is issued on request of the student/parent for official purposes.")
                            .FontSize(9).Italic().FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }

        public byte[] GenerateLeavingCertificate(StudentDto student)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(11).FontColor(Colors.Grey.Darken3));

                    page.Content().Column(col =>
                    {
                        // Header
                        col.Item().BorderBottom(2).BorderColor(Colors.Red.Darken2).PaddingBottom(8).Column(h =>
                        {
                            h.Item().AlignCenter().Text("SCHOOL MANAGEMENT SYSTEM")
                                .Bold().FontSize(18).FontColor(Colors.Red.Darken2);
                            h.Item().AlignCenter().Text("Leaving Certificate")
                                .Bold().FontSize(14).FontColor(Colors.Grey.Darken2);
                        });

                        col.Item().Height(24);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Certificate No: LC/{student.AdmissionNumber}/{DateTime.Today:yyyy}");
                            row.RelativeItem().AlignRight().Text($"Date of Issue: {DateTime.Today:dd/MM/yyyy}");
                        });

                        col.Item().Height(20);

                        col.Item().Text(text =>
                        {
                            text.Span("This is to certify that ");
                            text.Span($"{student.FirstName} {student.LastName}").Bold();
                            text.Span($" (Admission No: ");
                            text.Span(student.AdmissionNumber).Bold();
                            text.Span(") was a student of this institution.");
                        });

                        col.Item().Height(12);

                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                            });

                            void Row(string label, string value)
                            {
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text(label).SemiBold();
                                table.Cell().Padding(6).Text(value);
                            }

                            Row("Full Name", $"{student.FirstName} {student.LastName}");
                            Row("Date of Birth", student.DateOfBirth.ToString("dd/MM/yyyy"));
                            Row("Admission Number", student.AdmissionNumber);
                            Row("Admission Date", student.AdmissionDate.ToString("dd/MM/yyyy"));
                            Row("Last Class Studied", student.ClassName);
                            Row("Academic Year", student.AcademicYear);
                            if (!string.IsNullOrWhiteSpace(student.BloodGroup))
                                Row("Blood Group", student.BloodGroup);
                            if (!string.IsNullOrWhiteSpace(student.ParentName))
                                Row("Parent / Guardian", student.ParentName);
                        });

                        col.Item().Height(16);

                        col.Item().Text("The student's conduct and character during the period of study was satisfactory.");

                        col.Item().Height(40);

                        col.Item().AlignRight().Column(sig =>
                        {
                            sig.Item().Text("___________________________").FontColor(Colors.Grey.Darken1);
                            sig.Item().AlignRight().Text("Principal / Head of Institution").SemiBold();
                            sig.Item().AlignRight().Text("(Authorised Signatory)").FontColor(Colors.Grey.Medium);
                        });

                        col.Item().Height(20);

                        col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8)
                            .Text("This certificate is issued on request and is valid only with the official seal.")
                            .FontSize(9).Italic().FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }
    }
}
