using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.CustomResults;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Services;
using Project.Application.Common.Errors;
using Project.Application.DTOs.Expense;
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
        private readonly IExpenseService _expenseService;
        private readonly IExpenseExportService _expenseExportService;
        private readonly ILeaveService _leaveService;
        private readonly ILeaveExportService _leaveExportService;
        private readonly ISchoolSettingsService _settingsService;
        private readonly IWebHostEnvironment _env;

        public ReportsController(
            IStudentService studentService,
            IStudentExportService studentExportService,
            IExpenseService expenseService,
            IExpenseExportService expenseExportService,
            ILeaveService leaveService,
            ILeaveExportService leaveExportService,
            ISchoolSettingsService settingsService,
            IWebHostEnvironment env)
        {
            _studentService = studentService;
            _studentExportService = studentExportService;
            _expenseService = expenseService;
            _expenseExportService = expenseExportService;
            _leaveService = leaveService;
            _leaveExportService = leaveExportService;
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

        // ── Expenses ─────────────────────────────────────────────────────────

        [HttpGet("expenses")]
        public async Task<IActionResult> GetExpenses(
            [FromQuery] string? category,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate,
            CancellationToken ct)
        {
            var result = await _expenseService.GetReportDataAsync(
                new ExpenseFilterDto(category, fromDate, toDate), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            return Ok(ApiResponse<IReadOnlyList<ExpenseDto>>.Success(result.Value));
        }

        [HttpGet("expenses/export/pdf")]
        public async Task<IActionResult> ExportExpensesPdf(
            [FromQuery] string? category, [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate,
            CancellationToken ct)
        {
            var result = await _expenseService.GetReportDataAsync(
                new ExpenseFilterDto(category, fromDate, toDate), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_expenseExportService.ExportPdf(result.Value, category, fromDate, toDate, header),
                "application/pdf", "ExpensesReport.pdf");
        }

        [HttpGet("expenses/export/excel")]
        public async Task<IActionResult> ExportExpensesExcel(
            [FromQuery] string? category, [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate,
            CancellationToken ct)
        {
            var result = await _expenseService.GetReportDataAsync(
                new ExpenseFilterDto(category, fromDate, toDate), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_expenseExportService.ExportExcel(result.Value, category, fromDate, toDate, header),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExpensesReport.xlsx");
        }

        [HttpGet("expenses/export/word")]
        public async Task<IActionResult> ExportExpensesWord(
            [FromQuery] string? category, [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate,
            CancellationToken ct)
        {
            var result = await _expenseService.GetReportDataAsync(
                new ExpenseFilterDto(category, fromDate, toDate), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_expenseExportService.ExportWord(result.Value, category, fromDate, toDate, header),
                "application/msword", "ExpensesReport.doc");
        }

        // ── Leaves ───────────────────────────────────────────────────────────

        [HttpGet("leaves")]
        public async Task<IActionResult> GetLeaves(
            [FromQuery] string? status,
            [FromQuery] string? leaveType,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate,
            [FromQuery] int? staffId,
            CancellationToken ct)
        {
            var result = await _leaveService.GetReportDataAsync(
                new LeaveReportQueryDto(status, leaveType, fromDate, toDate, staffId), ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            return Ok(ApiResponse<IReadOnlyList<LeaveReportRowDto>>.Success(result.Value));
        }

        [HttpGet("leaves/export/pdf")]
        public async Task<IActionResult> ExportLeavesPdf(
            [FromQuery] string? status, [FromQuery] string? leaveType,
            [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate,
            [FromQuery] int? staffId,
            CancellationToken ct)
        {
            var query = new LeaveReportQueryDto(status, leaveType, fromDate, toDate, staffId);
            var result = await _leaveService.GetReportDataAsync(query, ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_leaveExportService.ExportPdf(result.Value, query, header),
                "application/pdf", "LeavesReport.pdf");
        }

        [HttpGet("leaves/export/excel")]
        public async Task<IActionResult> ExportLeavesExcel(
            [FromQuery] string? status, [FromQuery] string? leaveType,
            [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate,
            [FromQuery] int? staffId,
            CancellationToken ct)
        {
            var query = new LeaveReportQueryDto(status, leaveType, fromDate, toDate, staffId);
            var result = await _leaveService.GetReportDataAsync(query, ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_leaveExportService.ExportExcel(result.Value, query, header),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LeavesReport.xlsx");
        }

        [HttpGet("leaves/export/word")]
        public async Task<IActionResult> ExportLeavesWord(
            [FromQuery] string? status, [FromQuery] string? leaveType,
            [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate,
            [FromQuery] int? staffId,
            CancellationToken ct)
        {
            var query = new LeaveReportQueryDto(status, leaveType, fromDate, toDate, staffId);
            var result = await _leaveService.GetReportDataAsync(query, ct);
            if (result.IsFailure) return StatusCode(500, ApiResponse.Failure(result.Error.Message));
            var header = await BuildHeaderAsync(ct);
            return File(_leaveExportService.ExportWord(result.Value, query, header),
                "application/msword", "LeavesReport.doc");
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
