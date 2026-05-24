using Project.Application.Common.Result;
using Project.Application.DTOs.Document;
using Project.Application.DTOs.Report;
using Project.Application.DTOs.Student;
using Project.Application.Pagination;

namespace Project.Application.Abstractions.Services
{
    public interface IStudentService
    {
        Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto, CancellationToken ct = default);
        Task<Result<StudentDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<PagedList<StudentDto>>> GetAllAsync(StudentFilterDto filter, CancellationToken ct = default);
        Task<Result<StudentDto>> UpdateAsync(int id, UpdateStudentDto dto, CancellationToken ct = default);
        Task<Result> DeleteAsync(int id, CancellationToken ct = default);
        Task<Result<StudentDto>> UpdateProfileAsync(int studentId, UpdateStudentProfileDto dto, CancellationToken ct = default);
        Task<Result<IReadOnlyList<StudentDocumentDto>>> GetDocumentsAsync(int studentId, CancellationToken ct = default);
        Task<Result<StudentDocumentDto>> UploadDocumentAsync(int studentId, string documentType, string fileName, Stream fileStream, long fileSizeInBytes, CancellationToken ct = default);
        Task<Result<DownloadDocumentResult>> DownloadDocumentAsync(int studentId, int documentId, CancellationToken ct = default);
        Task<Result<GeneratedDocumentResult>> GenerateDocumentAsync(int studentId, string documentType, CancellationToken ct = default);
        Task<Result<IReadOnlyList<StudentReportRowDto>>> GetReportDataAsync(StudentReportQueryDto query, CancellationToken ct = default);
    }
}
