using FluentValidation;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Application.Common.Errors;
using Project.Application.Common.Result;
using Project.Application.DTOs.Document;
using Project.Application.DTOs.Report;
using Project.Application.DTOs.Student;
using Project.Application.Mapper;
using Project.Application.Pagination;
using Project.Application.Specifications.Students;
using Project.Domain.Aggregates.StudentAggregate;
using Project.Domain.Constants;
using Project.Domain.Enums;
using Project.Domain.ValueObjects;


namespace Project.Application.Services
{
    public sealed class StudentService : IStudentService
    {
        private readonly IRepository<Student> _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateStudentDto> _createValidator;
        private readonly IFileStorageService _fileStorage;
        private readonly IPdfGeneratorService _pdfGenerator;

        public StudentService(
            IRepository<Student> repository,
            IUnitOfWork unitOfWork,
            IValidator<CreateStudentDto> createValidator,
            IFileStorageService fileStorage,
            IPdfGeneratorService pdfGenerator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _fileStorage = fileStorage;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto, CancellationToken ct = default)
        {
            var validation = await _createValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
            {
                var messages = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
                return Result<StudentDto>.Failure(Error.Validation("Student.Validation", messages));
            }

            if (await _repository.ExistsAsync(s => s.AdmissionNumber == dto.AdmissionNumber && !s.IsDeleted, ct))
                return Result<StudentDto>.Failure(Error.Conflict("Student.AdmissionNumberExists",
                    $"Admission number '{dto.AdmissionNumber}' is already registered."));

            if (!Enum.TryParse<Gender>(dto.Gender, true, out var gender))
                return Result<StudentDto>.Failure(Error.Validation("Student.InvalidGender",
                    "Gender must be Male, Female, or Other."));

            var email = Email.Create(dto.Email);
            var phone = PhoneNumber.Create(dto.PhoneNumber);
            var address = Address.Create(dto.Street, dto.City, dto.State, dto.PinCode);

            var student = Student.Create(
                dto.FirstName,
                dto.LastName,
                email,
                phone,
                dto.DateOfBirth,
                gender,
                address,
                dto.AdmissionNumber,
                dto.AdmissionDate,
                dto.RollNumber,
                dto.ClassName,
                dto.AcademicYear,
                parentName: dto.ParentName,
                parentPhone: dto.ParentPhone,
                bloodGroup: dto.BloodGroup,
                parentEmail: dto.ParentEmail,
                emergencyContact: dto.EmergencyContact);

            await _repository.AddAsync(student, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<StudentDto>.Success(StudentMapper.ToDto(student));
        }

        public async Task<Result<StudentDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var spec = new StudentByIdSpecification(id);
            var student = await _repository.FirstOrDefaultAsync(spec, ct);

            if (student is null)
                return Result<StudentDto>.Failure(Error.NotFound("Student.NotFound",
                    $"Student with id {id} was not found."));

            return Result<StudentDto>.Success(StudentMapper.ToDto(student));
        }

        public async Task<Result<PagedList<StudentDto>>> GetAllAsync(StudentFilterDto filter, CancellationToken ct = default)
        {
            var countSpec = new StudentCountSpecification(filter);
            var totalCount = await _repository.CountAsync(countSpec, ct);

            var filterSpec = new StudentFilterSpecification(filter);
            var students = await _repository.ListAsync(filterSpec, ct);

            var dtos = students.Select(StudentMapper.ToDto).ToList();
            var paged = new PagedList<StudentDto>(dtos, totalCount, filter.PageNumber, filter.PageSize);

            return Result<PagedList<StudentDto>>.Success(paged);
        }

        public async Task<Result<StudentDto>> UpdateAsync(int id, UpdateStudentDto dto, CancellationToken ct = default)
        {
            var spec = new StudentByIdSpecification(id);
            var student = await _repository.FirstOrDefaultAsync(spec, ct);

            if (student is null)
                return Result<StudentDto>.Failure(Error.NotFound("Student.NotFound",
                    $"Student with id {id} was not found."));

            var phone = PhoneNumber.Create(dto.Phone);
            var address = Address.Create(dto.Street, dto.City, dto.State, dto.PinCode);

            student.Update(dto.FirstName, dto.LastName, phone, address,
                dto.RollNumber, dto.ClassName, dto.AcademicYear);

            _repository.Update(student);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<StudentDto>.Success(StudentMapper.ToDto(student));
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
        {
            var spec = new StudentByIdSpecification(id);
            var student = await _repository.FirstOrDefaultAsync(spec, ct);

            if (student is null)
                return Result.Failure(Error.NotFound("Student.NotFound",
                    $"Student with id {id} was not found."));

            _repository.Delete(student);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success();
        }

        public async Task<Result<StudentDto>> UpdateProfileAsync(
            int studentId, UpdateStudentProfileDto dto, CancellationToken ct = default)
        {
            var spec = new StudentByIdSpecification(studentId);
            var student = await _repository.FirstOrDefaultAsync(spec, ct);

            if (student is null)
                return Result<StudentDto>.Failure(Error.NotFound("Student.NotFound",
                    $"Student with id {studentId} was not found."));

            student.UpdateProfile(
                dto.BloodGroup,
                dto.ParentName,
                dto.ParentPhone,
                dto.ParentEmail,
                dto.EmergencyContact);

            _repository.Update(student);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<StudentDto>.Success(StudentMapper.ToDto(student));
        }

        public async Task<Result<IReadOnlyList<StudentDocumentDto>>> GetDocumentsAsync(
            int studentId, CancellationToken ct = default)
        {
            var spec = new StudentByIdSpecification(studentId);
            var student = await _repository.FirstOrDefaultAsync(spec, ct);

            if (student is null)
                return Result<IReadOnlyList<StudentDocumentDto>>.Failure(Error.NotFound("Student.NotFound",
                    $"Student with id {studentId} was not found."));

            var docs = student.Documents
                .Select(StudentMapper.ToDocumentDto)
                .ToList();

            return Result<IReadOnlyList<StudentDocumentDto>>.Success(docs);
        }

        public async Task<Result<StudentDocumentDto>> UploadDocumentAsync(
            int studentId, string documentType, string fileName,
            Stream fileStream, long fileSizeInBytes, CancellationToken ct = default)
        {
            var spec = new StudentByIdSpecification(studentId);
            var student = await _repository.FirstOrDefaultAsync(spec, ct);

            if (student is null)
                return Result<StudentDocumentDto>.Failure(Error.NotFound("Student.NotFound",
                    $"Student with id {studentId} was not found."));

            if (!Enum.TryParse<DocumentType>(documentType, true, out var docType))
                return Result<StudentDocumentDto>.Failure(Error.Validation("Student.InvalidDocumentType",
                    $"'{documentType}' is not a valid document type."));

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (!DocumentConstants.AllowedExtensions.Contains(ext))
                return Result<StudentDocumentDto>.Failure(Error.Validation("Student.InvalidFileType",
                    $"Allowed file types: {string.Join(", ", DocumentConstants.AllowedExtensions)}."));

            if (fileSizeInBytes > DocumentConstants.MaxFileSizeInBytes)
                return Result<StudentDocumentDto>.Failure(Error.Validation("Student.FileTooLarge",
                    $"File size must not exceed {DocumentConstants.MaxFileSizeInMb} MB."));

            var folder = $"students/{studentId}";
            var storedPath = await _fileStorage.UploadAsync(fileStream, fileName, folder, ct);

            var document = StudentDocument.Create(studentId, docType, fileName, storedPath, fileSizeInBytes);
            student.AddDocument(document);

            _repository.Update(student);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<StudentDocumentDto>.Success(StudentMapper.ToDocumentDto(document));
        }

        public async Task<Result<DownloadDocumentResult>> DownloadDocumentAsync(
            int studentId, int documentId, CancellationToken ct = default)
        {
            var spec = new StudentByIdSpecification(studentId);
            var student = await _repository.FirstOrDefaultAsync(spec, ct);

            if (student is null)
                return Result<DownloadDocumentResult>.Failure(Error.NotFound("Student.NotFound",
                    $"Student with id {studentId} was not found."));

            var document = student.Documents.FirstOrDefault(d => d.Id == documentId);
            if (document is null)
                return Result<DownloadDocumentResult>.Failure(Error.NotFound("Document.NotFound",
                    $"Document with id {documentId} was not found."));

            var stream = await _fileStorage.ReadAsync(document.FilePath, ct);
            if (stream is null)
                return Result<DownloadDocumentResult>.Failure(Error.NotFound("Document.FileNotFound",
                    "The file could not be found on the server."));

            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, ct);
            await stream.DisposeAsync();

            var ext = Path.GetExtension(document.FileName).ToLowerInvariant();
            var contentType = ext switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            return Result<DownloadDocumentResult>.Success(
                new DownloadDocumentResult(ms.ToArray(), document.FileName, contentType));
        }

        public async Task<Result<GeneratedDocumentResult>> GenerateDocumentAsync(
            int studentId, string documentType, CancellationToken ct = default)
        {
            var spec = new StudentByIdSpecification(studentId);
            var student = await _repository.FirstOrDefaultAsync(spec, ct);

            if (student is null)
                return Result<GeneratedDocumentResult>.Failure(Error.NotFound("Student.NotFound",
                    $"Student with id {studentId} was not found."));

            if (!Enum.TryParse<DocumentType>(documentType, true, out var docType))
                return Result<GeneratedDocumentResult>.Failure(Error.Validation("Student.InvalidDocumentType",
                    $"'{documentType}' is not a valid document type."));

            var studentDto = StudentMapper.ToDto(student);

            byte[] pdfBytes;
            string fileName;

            switch (docType)
            {
                case DocumentType.BonafideCertificate:
                    pdfBytes = _pdfGenerator.GenerateBonafideCertificate(studentDto);
                    fileName = $"Bonafide_{student.AdmissionNumber}.pdf";
                    break;
                case DocumentType.LeavingCertificate:
                    pdfBytes = _pdfGenerator.GenerateLeavingCertificate(studentDto);
                    fileName = $"LeavingCertificate_{student.AdmissionNumber}.pdf";
                    break;
                case DocumentType.StudentProfileReport:
                    pdfBytes = _pdfGenerator.GenerateStudentProfileReport(studentDto);
                    fileName = $"StudentReport_{student.AdmissionNumber}.pdf";
                    break;
                default:
                    return Result<GeneratedDocumentResult>.Failure(Error.Validation("Student.NotGeneratable",
                        $"'{documentType}' cannot be generated — it must be uploaded."));
            }

            return Result<GeneratedDocumentResult>.Success(
                new GeneratedDocumentResult(pdfBytes, fileName, "application/pdf"));
        }

        public async Task<Result<IReadOnlyList<StudentReportRowDto>>> GetReportDataAsync(
            StudentReportQueryDto query, CancellationToken ct = default)
        {
            var spec = new StudentReportSpecification(query);
            var students = await _repository.ListAsync(spec, ct);

            var rows = students
                .OrderBy(s => s.ClassName)
                .ThenBy(s => s.RollNumber)
                .Select(s => new StudentReportRowDto(
                    s.Id,
                    s.AdmissionNumber,
                    s.RollNumber,
                    $"{s.FirstName} {s.LastName}",
                    s.ClassName,
                    s.AcademicYear,
                    s.Gender.ToString(),
                    s.Phone.Value,
                    s.Email.Value,
                    s.ParentName,
                    s.ParentPhone))
                .ToList();

            return Result<IReadOnlyList<StudentReportRowDto>>.Success(rows);
        }
    }
}
