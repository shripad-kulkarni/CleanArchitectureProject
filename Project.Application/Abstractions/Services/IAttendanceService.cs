using Project.Application.Common.Result;
using Project.Application.DTOs.Attendance;

namespace Project.Application.Abstractions.Services
{
    public interface IAttendanceService
    {
        Task<Result<AttendanceDto>> MarkAttendanceAsync(MarkAttendanceDto dto, CancellationToken ct = default);
        Task<Result<AttendanceDto>> UpdateAttendanceAsync(int id, UpdateAttendanceDto dto, CancellationToken ct = default);
        Task<Result<AttendanceDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<AttendanceDto>> GetByStaffAndDateAsync(int staffId, DateOnly date, CancellationToken ct = default);
        Task<Result<IReadOnlyList<AttendanceDto>>> GetByStaffIdAsync(int staffId, CancellationToken ct = default);
        Task<Result<AttendanceReportDto>> GetMonthlyReportAsync(int staffId, int month, int year, CancellationToken ct = default);
        Task<Result<IReadOnlyList<AttendanceDto>>> BulkMarkAttendanceAsync(BulkMarkAttendanceDto dto, CancellationToken ct = default);
    }
}
