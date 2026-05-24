using Project.Application.Common.Result;
using Project.Application.DTOs.Salary;

namespace Project.Application.Abstractions.Services
{
    public interface ISalaryService
    {
        Task<Result<SalaryDto>> CreateAsync(CreateSalaryDto dto, CancellationToken ct = default);
        Task<Result<SalaryDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<IReadOnlyList<SalaryDto>>> GetByStaffIdAsync(int staffId, CancellationToken ct = default);
        Task<Result<SalaryDto>> MarkProcessedAsync(int id, CancellationToken ct = default);
        Task<Result<SalaryDto>> MarkPaidAsync(int id, CancellationToken ct = default);
        Task<Result> PutOnHoldAsync(int id, CancellationToken ct = default);
    }
}
