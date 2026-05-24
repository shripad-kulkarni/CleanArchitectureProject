using Project.Application.Common.Result;
using Project.Application.DTOs.Expense;
using Project.Application.Pagination;

namespace Project.Application.Abstractions.Services
{
    public interface IExpenseService
    {
        Task<Result<ExpenseDto>> CreateAsync(CreateExpenseDto dto, CancellationToken ct = default);
        Task<Result<ExpenseDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<PagedList<ExpenseDto>>> GetAllAsync(ExpenseFilterDto filter, CancellationToken ct = default);
        Task<Result<ExpenseDto>> UpdateAsync(int id, UpdateExpenseDto dto, CancellationToken ct = default);
        Task<Result> DeleteAsync(int id, CancellationToken ct = default);
        Task<Result<IReadOnlyList<ExpenseDto>>> GetReportDataAsync(ExpenseFilterDto filter, CancellationToken ct = default);
    }
}
