using Project.Application.Common.Result;
using Project.Application.DTOs.Salary;
using Project.Application.DTOs.Staff;
using Project.Application.Pagination;

namespace Project.Application.Abstractions.Services
{
    public interface IStaffService
    {
        Task<Result<StaffDto>> CreateAsync(CreateStaffDto dto, CancellationToken ct = default);
        Task<Result<StaffDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<PagedList<StaffDto>>> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken ct = default);
        Task<Result<StaffDto>> UpdateAsync(int id, UpdateStaffDto dto, CancellationToken ct = default);
        Task<Result> DeleteAsync(int id, CancellationToken ct = default);
        Task<Result<StaffDto>> UpdateSalaryAsync(int staffId, UpdateStaffSalaryDto dto, CancellationToken ct = default);
        Task<Result<IReadOnlyList<SalaryIncrementDto>>> GetSalaryHistoryAsync(int staffId, CancellationToken ct = default);
        Task<Result<StaffDto>> GetByEmailAsync(string email, CancellationToken ct = default);
    }
}
