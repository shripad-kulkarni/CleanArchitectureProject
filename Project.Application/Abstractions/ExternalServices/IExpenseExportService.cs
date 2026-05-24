using Project.Application.DTOs.Expense;
using Project.Application.DTOs.Settings;

namespace Project.Application.Abstractions.ExternalServices
{
    public interface IExpenseExportService
    {
        byte[] ExportPdf(IReadOnlyList<ExpenseDto> rows, string? category, DateOnly? fromDate, DateOnly? toDate, SchoolHeaderDto? header = null);
        byte[] ExportExcel(IReadOnlyList<ExpenseDto> rows, string? category, DateOnly? fromDate, DateOnly? toDate, SchoolHeaderDto? header = null);
        byte[] ExportWord(IReadOnlyList<ExpenseDto> rows, string? category, DateOnly? fromDate, DateOnly? toDate, SchoolHeaderDto? header = null);
    }
}
