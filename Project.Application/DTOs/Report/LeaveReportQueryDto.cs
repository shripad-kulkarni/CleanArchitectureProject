namespace Project.Application.DTOs.Report
{
    public record LeaveReportQueryDto(
        string? Status = null,
        string? LeaveType = null,
        DateOnly? FromDate = null,
        DateOnly? ToDate = null,
        int? StaffId = null);
}
