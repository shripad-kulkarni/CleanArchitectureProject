using Project.Application.Common.Result;
using Project.Application.DTOs.Fee;

namespace Project.Application.Abstractions.Services
{
    public interface IFeeService
    {
        Task<Result<FeeDto>> CreateAsync(CreateFeeDto dto, CancellationToken ct = default);
        Task<Result<FeeDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<IReadOnlyList<FeeDto>>> GetByStudentIdAsync(int studentId, CancellationToken ct = default);
        Task<Result<FeeDto>> CollectPaymentAsync(CollectFeeDto dto, CancellationToken ct = default);
        Task<Result<FeeDto>> AddInstallmentAsync(AddInstallmentDto dto, CancellationToken ct = default);
        Task<Result> WaiveFeeAsync(int feeId, CancellationToken ct = default);
        Task<Result> MarkOverdueAsync(int feeId, CancellationToken ct = default);
    }
}
