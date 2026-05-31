using DinkToPdf;
using DinkToPdf.Contracts;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.DTOs.User;

namespace Project.Infrastructure.Services
{
    public sealed class UserProfileReportService : IUserProfileReportService
    {
        private readonly RazorViewRenderer _renderer;
        private readonly IConverter _converter;

        public UserProfileReportService(RazorViewRenderer renderer, IConverter converter)
        {
            _renderer = renderer;
            _converter = converter;
        }

        public async Task<byte[]> GenerateAsync(UserDto user, CancellationToken ct = default)
        {
            var html = await _renderer.RenderAsync("Reports/UserProfileReport", user);

            var document = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings
                    {
                        Top = 0, Bottom = 0, Left = 0, Right = 0,
                        Unit = Unit.Millimeters
                    },
                    DocumentTitle = $"User Profile — {user.FullName}",
                },
                Objects =
                {
                    new ObjectSettings
                    {
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" },
                    }
                }
            };

            return _converter.Convert(document);
        }
    }
}
