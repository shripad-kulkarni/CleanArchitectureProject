namespace Project.Application.DTOs.Report
{
    public record LeaveReportRowDto(
        int Id,
        int StaffId,
        string StaffName,
        string LeaveType,
        DateOnly StartDate,
        DateOnly EndDate,
        int TotalDays,
        string Status,
        string Reason,
        string? RejectionReason);
}
