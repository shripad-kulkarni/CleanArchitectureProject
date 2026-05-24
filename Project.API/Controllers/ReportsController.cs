using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.CustomResults;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Services;
using Project.Application.Common.Errors; 
using Project.Application.DTOs.Report;
using Project.Application.DTOs.Settings;

namespace Project.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/reports")]
    [Authorize]
    public sealed class ReportsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IStudentExportService _studentExportService; 
        private readonly ISchoolSettingsService _settingsService;
        private readonly IWebHostEnvironment _env;

        public ReportsController(
            IStudentService studentService,
            IStudentExportService studentExportService, 
            ISchoolSettingsService settingsService,
            IWebHostEnvironment env)
        {
            _studentService = studentService;
            _studentExportService = studentExportService; 
            _settingsService = settingsService;
            _env = env;
        }

        // ── Students ─────────────────────────────────────────────────────────

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents(
            [FromQuery] string? className, [FromQuery] string? academicYear, CancellationToken ct)
        {
            var result = await _studentService.GetReportDataAsync(
                new StudentReportQueryDto(className, academicYear), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            return Ok(ApiResponse<IReadOnlyList<StudentReportRowDto>>.Success(result.Value));
        }

        [HttpGet("students/export/pdf")]
        public async Task<IActionResult> ExportStudentsPdf(
            [FromQuery] string? className, [FromQuery] string? academicYear, CancellationToken ct)
        {
            var result = await _studentService.GetReportDataAsync(
                new StudentReportQueryDto(className, academicYear), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_studentExportService.ExportPdf(result.Value, className, academicYear, header),
                "application/pdf", "StudentsReport.pdf");
        }

        [HttpGet("students/export/excel")]
        public async Task<IActionResult> ExportStudentsExcel(
            [FromQuery] string? className, [FromQuery] string? academicYear, CancellationToken ct)
        {
            var result = await _studentService.GetReportDataAsync(
                new StudentReportQueryDto(className, academicYear), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_studentExportService.ExportExcel(result.Value, className, academicYear, header),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentsReport.xlsx");
        }

        [HttpGet("students/export/word")]
        public async Task<IActionResult> ExportStudentsWord(
            [FromQuery] string? className, [FromQuery] string? academicYear, CancellationToken ct)
        {
            var result = await _studentService.GetReportDataAsync(
                new StudentReportQueryDto(className, academicYear), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_studentExportService.ExportWord(result.Value, className, academicYear, header),
                "application/msword", "StudentsReport.doc");
        }

         
        // ── Helpers ───────────────────────────────────────────────────────────

        private async Task<SchoolHeaderDto?> BuildHeaderAsync(CancellationToken ct)
        {
            try
            {
                var settings = await _settingsService.GetAsync(ct);
                byte[]? logoBytes = null;
                if (!string.IsNullOrWhiteSpace(settings.LogoPath))
                {
                    var physicalPath = Path.Combine(_env.ContentRootPath, "wwwroot",
                        settings.LogoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(physicalPath))
                        logoBytes = await System.IO.File.ReadAllBytesAsync(physicalPath, ct);
                }
                return new SchoolHeaderDto(settings.SchoolName, settings.Address, settings.PhoneNumber, logoBytes);
            }
            catch
            {
                return null;
            }
        }

        private IActionResult ToErrorResponse(Error error) => error.Type switch
        {
            ErrorType.NotFound => NotFound(ApiResponse.Failure(error.Message)),
            _ => StatusCode(500, ApiResponse.Failure(error.Message))
        };
    }
}
