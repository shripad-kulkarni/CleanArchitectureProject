using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.Controllers.Base;
using Project.API.CustomResults;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Services;
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
        private readonly IUserExportService _userExportService;
        private readonly IInfoSettingsService _settingsService;

        public ReportsController(
            IUserService userService,
            IUserExportService userExportService,
            IInfoSettingsService settingsService)
        {
            _userService = userService;
            _userExportService = userExportService;
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
            var header = await BuildHeaderAsync(ct);
            return File(_userExportService.ExportPdf(result.Value, BuildFilterLabel(searchTerm, gender), header),
                "application/pdf", "UsersReport.pdf");
        }

        [HttpGet("users/export/excel")]
        public async Task<IActionResult> ExportUsersExcel(
            [FromQuery] string? searchTerm, [FromQuery] string? gender, CancellationToken ct)
        {
            var result = await _userService.GetReportDataAsync(new UserReportQueryDto(searchTerm, gender), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_userExportService.ExportExcel(result.Value, BuildFilterLabel(searchTerm, gender), header),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UsersReport.xlsx");
        }

        [HttpGet("users/export/word")]
        public async Task<IActionResult> ExportUsersWord(
            [FromQuery] string? searchTerm, [FromQuery] string? gender, CancellationToken ct)
        {
            var result = await _userService.GetReportDataAsync(new UserReportQueryDto(searchTerm, gender), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_userExportService.ExportWord(result.Value, BuildFilterLabel(searchTerm, gender), header),
                "application/msword", "UsersReport.doc");
        }

        private async Task<InfoHeaderDto?> BuildHeaderAsync(CancellationToken ct)
        {
            try
            {
                var settings = await _settingsService.GetAsync(ct);
                return new InfoHeaderDto(settings.SchoolName, settings.Address, settings.PhoneNumber, null);
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
