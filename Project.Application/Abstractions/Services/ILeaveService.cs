using Project.Application.Common.Result;
using Project.Application.DTOs.Leave;
using Project.Application.DTOs.Report;

namespace Project.Application.Abstractions.Services
{
    public interface ILeaveService
    {
        Task<Result<LeaveDto>> CreateAsync(CreateLeaveDto dto, CancellationToken ct = default);
        Task<Result<LeaveDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<IReadOnlyList<LeaveDto>>> GetByStaffIdAsync(int staffId, CancellationToken ct = default);
        Task<Result<IReadOnlyList<PendingLeaveDto>>> GetPendingAsync(CancellationToken ct = default);
        Task<Result<LeaveDto>> ApproveAsync(int id, ApproveLeaveDto dto, CancellationToken ct = default);
        Task<Result<LeaveDto>> RejectAsync(int id, RejectLeaveDto dto, CancellationToken ct = default);
        Task<Result> CancelAsync(int id, CancellationToken ct = default);
        Task<Result<IReadOnlyList<LeaveReportRowDto>>> GetReportDataAsync(LeaveReportQueryDto query, CancellationToken ct = default);
    }
}
