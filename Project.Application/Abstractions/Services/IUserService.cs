using Project.Application.Common.Result;
using Project.Application.DTOs.Document;
using Project.Application.DTOs.Report;
using Project.Application.DTOs.User;
using Project.Application.Pagination;

namespace Project.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<Result<UserDto>> CreateAsync(CreateUserDto dto,
            Stream? profilePhotoStream = null, string? profilePhotoFileName = null,
            Stream? introVideoStream = null, string? introVideoFileName = null,
            CancellationToken ct = default);
        Task<Result<UserDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<PagedList<UserDto>>> GetAllAsync(UserFilterDto filter, CancellationToken ct = default);
        Task<Result<UserDto>> UpdateAsync(int id, UpdateUserDto dto,
            Stream? profilePhotoStream = null, string? profilePhotoFileName = null,
            Stream? introVideoStream = null, string? introVideoFileName = null,
            CancellationToken ct = default);
        Task<Result> DeleteAsync(int id, CancellationToken ct = default);
        Task<Result<UserDto>> UpdateProfileAsync(int userId, UpdateUserProfileDto dto, CancellationToken ct = default);
        Task<Result<IReadOnlyList<UserDocumentDto>>> GetDocumentsAsync(int userId, CancellationToken ct = default);
        Task<Result<UserDocumentDto>> UploadDocumentAsync(int userId, string documentType, string fileName, Stream fileStream, long fileSizeInBytes, CancellationToken ct = default);
        Task<Result<DownloadDocumentResult>> DownloadDocumentAsync(int userId, int documentId, CancellationToken ct = default);
        Task<Result<GeneratedDocumentResult>> GenerateDocumentAsync(int userId, string documentType, CancellationToken ct = default);
        Task<Result<IReadOnlyList<UserReportRowDto>>> GetReportDataAsync(UserReportQueryDto query, CancellationToken ct = default);
    }
}
