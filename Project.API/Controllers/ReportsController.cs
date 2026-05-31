using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers.Base;
using Project.API.CustomResults;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.Export;
using Project.Application.DTOs.Report;
using Project.Application.DTOs.Settings;

namespace Project.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/reports")]
    [Authorize]
    public sealed class ReportsController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IExportService _exportService;
        private readonly IInfoSettingsService _settingsService;

        public ReportsController(
            IUserService userService,
            IExportService exportService,
            IInfoSettingsService settingsService)
        {
            _userService = userService;
            _exportService = exportService;
            _settingsService = settingsService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? searchTerm, [FromQuery] string? gender, CancellationToken ct)
        {
            var result = await _userService.GetReportDataAsync(new UserReportQueryDto(searchTerm, gender), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            return Ok(ApiResponse<IReadOnlyList<UserReportRowDto>>.Success(result.Value));
        }

        [HttpGet("users/export/pdf")]
        public async Task<IActionResult> ExportUsersPdf(
            [FromQuery] string? searchTerm, [FromQuery] string? gender, CancellationToken ct)
        {
            var result = await _userService.GetReportDataAsync(new UserReportQueryDto(searchTerm, gender), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var options = await BuildUserExportOptionsAsync(searchTerm, gender, ct);
            return File(_exportService.ExportPdf(result.Value, options), "application/pdf", "UsersReport.pdf");
        }

        [HttpGet("users/export/excel")]
        public async Task<IActionResult> ExportUsersExcel(
            [FromQuery] string? searchTerm, [FromQuery] string? gender, CancellationToken ct)
        {
            var result = await _userService.GetReportDataAsync(new UserReportQueryDto(searchTerm, gender), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var options = await BuildUserExportOptionsAsync(searchTerm, gender, ct);
            return File(_exportService.ExportExcel(result.Value, options),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UsersReport.xlsx");
        }

        [HttpGet("users/export/word")]
        public async Task<IActionResult> ExportUsersWord(
            [FromQuery] string? searchTerm, [FromQuery] string? gender, CancellationToken ct)
        {
            var result = await _userService.GetReportDataAsync(new UserReportQueryDto(searchTerm, gender), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var options = await BuildUserExportOptionsAsync(searchTerm, gender, ct);
            return File(_exportService.ExportWord(result.Value, options), "application/msword", "UsersReport.doc");
        }

        private async Task<ExportOptions<UserReportRowDto>> BuildUserExportOptionsAsync(
            string? searchTerm, string? gender, CancellationToken ct)
        {
            var header = await BuildHeaderAsync(ct);
            return new ExportOptions<UserReportRowDto>
            {
                ReportTitle = "Users Report",
                FilterLabel = BuildFilterLabel(searchTerm, gender) ?? "All Users",
                Header = header,
                TotalLabel = "Total Users",
                Columns =
                [
                    new("#",           (_, i) => (i + 1).ToString(),  ConstantWidth: 28),
                    new("Name",        (r, _) => r.FullName,           RelativeWidth: 3),
                    new("Gender",      (r, _) => r.Gender,             ConstantWidth: 50),
                    new("Email",       (r, _) => r.Email,              RelativeWidth: 3),
                    new("Phone",       (r, _) => r.Phone,              RelativeWidth: 2),
                    new("Blood Group", (r, _) => r.BloodGroup ?? "",   RelativeWidth: 2),
                ]
            };
        }

        private async Task<InfoHeaderDto?> BuildHeaderAsync(CancellationToken ct)
        {
            try
            {
                var settings = await _settingsService.GetAsync(ct);
                return new InfoHeaderDto(settings.Name, settings.Address, settings.PhoneNumber, null);
            }
            catch
            {
                return null;
            }
        }

        private static string? BuildFilterLabel(string? searchTerm, string? gender)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(searchTerm)) parts.Add($"Search: {searchTerm}");
            if (!string.IsNullOrWhiteSpace(gender)) parts.Add($"Gender: {gender}");
            return parts.Count > 0 ? string.Join("   |   ", parts) : null;
        }
    }
}
