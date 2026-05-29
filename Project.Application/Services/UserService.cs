using FluentValidation;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Application.Common.Errors;
using Project.Application.Common.Result;
using Project.Application.DTOs.Document;
using Project.Application.DTOs.Report;
using Project.Application.DTOs.User;
using Project.Application.Mapper;
using Project.Application.Pagination;
using Project.Application.Specifications.Users;
using Project.Domain.Aggregates.UserAggregate;
using Project.Domain.Constants;
using Project.Domain.Enums;
using Project.Domain.ValueObjects;

namespace Project.Application.Services
{
    public sealed class UserService : IUserService
    {
        private readonly IRepository<User> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateUserDto> _createValidator;
        private readonly IFileStorageService _fileStorage;
        private readonly IPdfGeneratorService _pdfGenerator;

        public UserService(
            IRepository<User> repository,
            IUnitOfWork unitOfWork,
            IValidator<CreateUserDto> createValidator,
            IFileStorageService fileStorage,
            IPdfGeneratorService pdfGenerator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _fileStorage = fileStorage;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<Result<UserDto>> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
        {
            var validation = await _createValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
            {
                var messages = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                return Result<UserDto>.Failure(Error.Validation("User.Validation", messages));
            }

            if (await _repository.ExistsAsync(u => u.Email.Value == dto.Email && !u.IsDeleted, ct))
                return Result<UserDto>.Failure(Error.Conflict("User.EmailExists",
                    $"Email '{dto.Email}' is already registered."));

            if (!Enum.TryParse<Gender>(dto.Gender, true, out var gender))
                return Result<UserDto>.Failure(Error.Validation("User.InvalidGender",
                    "Gender must be Male, Female, or Other."));

            var email = Email.Create(dto.Email);
            var phone = PhoneNumber.Create(dto.PhoneNumber);
            var address = Address.Create(dto.Street, dto.City, dto.State, dto.PinCode);

            var user = User.Create(
                dto.FirstName, dto.LastName, email, phone,
                dto.DateOfBirth, gender, address,
                bloodGroup: dto.BloodGroup,
                emergencyContact: dto.EmergencyContact);

            await _repository.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<UserDto>.Success(UserMapper.ToDto(user));
        }

        public async Task<Result<UserDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var user = await _repository.FirstOrDefaultAsync(new UserByIdSpecification(id), ct);
            if (user is null)
                return Result<UserDto>.Failure(Error.NotFound("User.NotFound", $"User with id {id} was not found."));

            return Result<UserDto>.Success(UserMapper.ToDto(user));
        }

        public async Task<Result<PagedList<UserDto>>> GetAllAsync(UserFilterDto filter, CancellationToken ct = default)
        {
            var totalCount = await _repository.CountAsync(new UserCountSpecification(filter), ct);
            var users = await _repository.ListAsync(new UserFilterSpecification(filter), ct);
            var dtos = users.Select(UserMapper.ToDto).ToList();
            return Result<PagedList<UserDto>>.Success(new PagedList<UserDto>(dtos, totalCount, filter.PageNumber, filter.PageSize));
        }

        public async Task<Result<UserDto>> UpdateAsync(int id, UpdateUserDto dto, CancellationToken ct = default)
        {
            var user = await _repository.FirstOrDefaultAsync(new UserByIdSpecification(id), ct);
            if (user is null)
                return Result<UserDto>.Failure(Error.NotFound("User.NotFound", $"User with id {id} was not found."));

            user.Update(dto.FirstName, dto.LastName,
                PhoneNumber.Create(dto.Phone),
                Address.Create(dto.Street, dto.City, dto.State, dto.PinCode));

            _repository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<UserDto>.Success(UserMapper.ToDto(user));
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
        {
            var user = await _repository.FirstOrDefaultAsync(new UserByIdSpecification(id), ct);
            if (user is null)
                return Result.Failure(Error.NotFound("User.NotFound", $"User with id {id} was not found."));

            _repository.Delete(user);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }

        public async Task<Result<UserDto>> UpdateProfileAsync(int userId, UpdateUserProfileDto dto, CancellationToken ct = default)
        {
            var user = await _repository.FirstOrDefaultAsync(new UserByIdSpecification(userId), ct);
            if (user is null)
                return Result<UserDto>.Failure(Error.NotFound("User.NotFound", $"User with id {userId} was not found."));

            user.UpdateProfile(dto.BloodGroup, dto.EmergencyContact);

            _repository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<UserDto>.Success(UserMapper.ToDto(user));
        }

        public async Task<Result<IReadOnlyList<UserDocumentDto>>> GetDocumentsAsync(int userId, CancellationToken ct = default)
        {
            var user = await _repository.FirstOrDefaultAsync(new UserByIdSpecification(userId), ct);
            if (user is null)
                return Result<IReadOnlyList<UserDocumentDto>>.Failure(Error.NotFound("User.NotFound", $"User with id {userId} was not found."));

            var docs = user.Documents.Select(UserMapper.ToDocumentDto).ToList();
            return Result<IReadOnlyList<UserDocumentDto>>.Success(docs);
        }

        public async Task<Result<UserDocumentDto>> UploadDocumentAsync(
            int userId, string documentType, string fileName,
            Stream fileStream, long fileSizeInBytes, CancellationToken ct = default)
        {
            var user = await _repository.FirstOrDefaultAsync(new UserByIdSpecification(userId), ct);
            if (user is null)
                return Result<UserDocumentDto>.Failure(Error.NotFound("User.NotFound", $"User with id {userId} was not found."));

            if (!Enum.TryParse<DocumentType>(documentType, true, out var docType))
                return Result<UserDocumentDto>.Failure(Error.Validation("User.InvalidDocumentType",
                    $"'{documentType}' is not a valid document type."));

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (!DocumentConstants.AllowedExtensions.Contains(ext))
                return Result<UserDocumentDto>.Failure(Error.Validation("User.InvalidFileType",
                    $"Allowed file types: {string.Join(", ", DocumentConstants.AllowedExtensions)}."));

            if (fileSizeInBytes > DocumentConstants.MaxFileSizeInBytes)
                return Result<UserDocumentDto>.Failure(Error.Validation("User.FileTooLarge",
                    $"File size must not exceed {DocumentConstants.MaxFileSizeInMb} MB."));

            var storedPath = await _fileStorage.UploadAsync(fileStream, fileName, $"users/{userId}", ct);
            var document = UserDocument.Create(userId, docType, fileName, storedPath, fileSizeInBytes);
            user.AddDocument(document);

            _repository.Update(user);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<UserDocumentDto>.Success(UserMapper.ToDocumentDto(document));
        }

        public async Task<Result<DownloadDocumentResult>> DownloadDocumentAsync(
            int userId, int documentId, CancellationToken ct = default)
        {
            var user = await _repository.FirstOrDefaultAsync(new UserByIdSpecification(userId), ct);
            if (user is null)
                return Result<DownloadDocumentResult>.Failure(Error.NotFound("User.NotFound", $"User with id {userId} was not found."));

            var document = user.Documents.FirstOrDefault(d => d.Id == documentId);
            if (document is null)
                return Result<DownloadDocumentResult>.Failure(Error.NotFound("Document.NotFound", $"Document with id {documentId} was not found."));

            var stream = await _fileStorage.ReadAsync(document.FilePath, ct);
            if (stream is null)
                return Result<DownloadDocumentResult>.Failure(Error.NotFound("Document.FileNotFound", "The file could not be found on the server."));

            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, ct);
            await stream.DisposeAsync();

            var contentType = Path.GetExtension(document.FileName).ToLowerInvariant() switch
            {
                ".pdf"            => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png"            => "image/png",
                _                 => "application/octet-stream"
            };

            return Result<DownloadDocumentResult>.Success(
                new DownloadDocumentResult(ms.ToArray(), document.FileName, contentType));
        }

        public async Task<Result<GeneratedDocumentResult>> GenerateDocumentAsync(
            int userId, string documentType, CancellationToken ct = default)
        {
            var user = await _repository.FirstOrDefaultAsync(new UserByIdSpecification(userId), ct);
            if (user is null)
                return Result<GeneratedDocumentResult>.Failure(Error.NotFound("User.NotFound", $"User with id {userId} was not found."));

            if (!Enum.TryParse<DocumentType>(documentType, true, out var docType))
                return Result<GeneratedDocumentResult>.Failure(Error.Validation("User.InvalidDocumentType",
                    $"'{documentType}' is not a valid document type."));

            if (docType is not DocumentType.ProfileReport)
                return Result<GeneratedDocumentResult>.Failure(Error.Validation("User.NotGeneratable",
                    $"'{documentType}' cannot be generated — it must be uploaded."));

            var userDto = UserMapper.ToDto(user);
            var pdfBytes = _pdfGenerator.GenerateUserProfileReport(userDto);

            return Result<GeneratedDocumentResult>.Success(
                new GeneratedDocumentResult(pdfBytes, $"UserReport_{user.Id}.pdf", "application/pdf"));
        }

        public async Task<Result<IReadOnlyList<UserReportRowDto>>> GetReportDataAsync(
            UserReportQueryDto query, CancellationToken ct = default)
        {
            var users = await _repository.ListAsync(new UserReportSpecification(query), ct);

            var rows = users
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserReportRowDto(
                    u.Id,
                    $"{u.FirstName} {u.LastName}",
                    u.Gender.ToString(),
                    u.Phone.Value,
                    u.Email.Value,
                    u.BloodGroup,
                    u.EmergencyContact))
                .ToList();

            return Result<IReadOnlyList<UserReportRowDto>>.Success(rows);
        }
    }
}
