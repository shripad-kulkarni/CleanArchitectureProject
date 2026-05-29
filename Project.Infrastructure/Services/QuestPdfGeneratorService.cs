using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.DTOs.User;

namespace Project.Infrastructure.Services
{
    public sealed class QuestPdfGeneratorService : IPdfGeneratorService
    {
        public byte[] GenerateUserProfileReport(UserDto user)
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
                        col.Item().BorderBottom(2).BorderColor(Colors.Blue.Darken2).PaddingBottom(8).Column(h =>
                        {
                            h.Item().AlignCenter().Text("USER PROFILE REPORT")
                                .Bold().FontSize(18).FontColor(Colors.Blue.Darken2);
                        });

                        col.Item().Height(16);

                        col.Item().AlignRight().Text($"Generated: {DateTime.Today:dd/MM/yyyy}")
                            .FontSize(9).FontColor(Colors.Grey.Medium);

                        col.Item().Height(12);

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

                            Cell("Full Name", user.FullName);
                            Cell("Gender", user.Gender);
                            Cell("Date of Birth", user.DateOfBirth.ToString("dd/MM/yyyy"));
                            Cell("Blood Group", string.IsNullOrWhiteSpace(user.BloodGroup) ? "—" : user.BloodGroup);
                        });

                        col.Item().Height(12);

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

                            Cell("Email", user.Email);
                            Cell("Phone", user.Phone);
                            Cell("City", user.City);
                            Cell("State", user.State);
                            Cell("Street", user.Street);
                            Cell("Pin Code", user.PinCode);
                        });

                        if (!string.IsNullOrWhiteSpace(user.EmergencyContact))
                        {
                            col.Item().Height(12);

                            col.Item().Background(Colors.Blue.Darken2).Padding(6)
                                .Text("Emergency Contact").Bold().FontSize(11).FontColor(Colors.White);

                            col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(8);
                                });

                                table.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Contact").SemiBold().FontSize(10);
                                table.Cell().Padding(6).Text(user.EmergencyContact).FontSize(10);
                            });
                        }

                        col.Item().Height(40);

                        col.Item().AlignRight().Column(sig =>
                        {
                            sig.Item().Text("___________________________").FontColor(Colors.Grey.Darken1);
                            sig.Item().AlignRight().Text("Authorised Signatory").SemiBold();
                        });

                        col.Item().Height(20);

                        col.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingTop(8)
                            .Text("This report is system-generated and is valid for official reference purposes.")
                            .FontSize(9).Italic().FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }
    }
}
