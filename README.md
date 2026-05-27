# CrsJourney — School Management API

> ASP.NET Core 8 · Clean Architecture · MySQL · JWT · QuestPDF

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Project.API  (Presentation)                  │
│                                                                       │
│   Controllers  ·  ApiResponse<T>  ·  GlobalExceptionHandler          │
│   Swagger  ·  API Versioning  ·  Serilog  ·  CORS                   │
└────────────────────────────┬────────────────────────────────────────┘
                             │ depends on
┌────────────────────────────▼────────────────────────────────────────┐
│                    Project.Infrastructure                             │
│                                                                       │
│   AppDbContext (EF Core + MySQL)   ·   GenericRepository<T>          │
│   UnitOfWork   ·   EF Interceptors (Audit, SoftDelete)               │
│   Identity / JWT   ·   Email / FileStorage / PDF / Export            │
│   Migrations  ·  Seeding  ·  Options                                 │
└────────────────────────────┬────────────────────────────────────────┘
                             │ depends on
┌────────────────────────────▼────────────────────────────────────────┐
│                     Project.Application  (Use Cases)                  │
│                                                                       │
│   Service interfaces  ·  Concrete Services                           │
│   DTOs  ·  Mappers  ·  Validators (FluentValidation)                 │
│   Specifications  ·  Pagination  ·  Result / Error pattern           │
│   IRepository<T>  ·  IUnitOfWork  ·  ICurrentUserService             │
└────────────────────────────┬────────────────────────────────────────┘
                             │ depends on
┌────────────────────────────▼────────────────────────────────────────┐
│                     Project.Domain  (Core Business)                   │
│                                                                       │
│   Aggregates  ·  Value Objects  ·  Enums  ·  Constants               │
│   Domain Exceptions  ·  Entity / AggregateRoot primitives            │
└─────────────────────────────────────────────────────────────────────┘
```

> **Dependency Rule**: arrows point inward only. Domain knows nothing about any outer layer.

---

## Project Structure

```
CleanArchitectureProject/
│
├── Project.Domain/
│   ├── Primitives/
│   │   ├── Entity.cs                     # Base class — Id + audit fields + soft-delete
│   │   └── AggregateRoot.cs              # Marker base for aggregate roots
│   │
│   ├── Aggregates/
│   │   ├── StudentAggregate/
│   │   │   ├── Student.cs                # AggregateRoot — factory Create(), Update()
│   │   │   └── StudentDocument.cs        # Child entity of Student
│   │   ├── StaffAggregate/Staff.cs
│   │   ├── StaffAttendanceAggregates/StaffAttendance.cs
│   │   ├── StaffLeaveAggregate/StaffLeave.cs
│   │   ├── FeeAggregate/
│   │   │   ├── Fee.cs
│   │   │   └── FeeInstallment.cs
│   │   ├── SalaryAggregate/
│   │   │   ├── Salary.cs
│   │   │   └── SalaryIncrement.cs
│   │   ├── ExpenseAggregate/Expense.cs
│   │   ├── MenuSettingAggregate/MenuSetting.cs
│   │   └── SchoolSettingAggregate/SchoolSettings.cs
│   │
│   ├── ValueObjects/
│   │   ├── Email.cs                      # Validated, immutable — Create() factory
│   │   ├── PhoneNumber.cs
│   │   ├── Address.cs
│   │   ├── Money.cs                      # Amount + Currency, Add/Subtract
│   │   └── DateRange.cs
│   │
│   ├── Enums/
│   │   ├── Gender.cs
│   │   ├── DocumentType.cs
│   │   ├── FeeStatus.cs
│   │   ├── LeaveStatus.cs / LeaveType.cs
│   │   ├── SalaryStatus.cs
│   │   ├── StaffRole.cs
│   │   └── AttendanceStatus.cs
│   │
│   ├── Constants/
│   │   ├── DocumentConstants.cs          # Allowed extensions, max file size
│   │   ├── FeeConstants.cs
│   │   └── RoleConstants.cs              # "Admin", "Staff", etc.
│   │
│   └── Exceptions/
│       ├── DomainException.cs
│       ├── InvalidFeeOperationException.cs
│       ├── InvalidLeaveRequestException.cs
│       ├── StaffNotFoundException.cs
│       └── StudentNotFoundException.cs
│
├── Project.Application/
│   ├── Abstractions/
│   │   ├── Persistence/
│   │   │   ├── IRepository.cs            # Generic CRUD + spec queries
│   │   │   ├── IUnitOfWork.cs            # SaveChangesAsync
│   │   │   └── IMenuSettingRepository.cs # Custom repo interface
│   │   ├── Identity/
│   │   │   └── ICurrentUserService.cs    # UserId, UserName, Role
│   │   ├── ExternalServices/
│   │   │   ├── IDateTimeProvider.cs
│   │   │   ├── IEmailService.cs
│   │   │   ├── IFileStorageService.cs
│   │   │   ├── IPdfGeneratorService.cs
│   │   │   └── IStudentExportService.cs
│   │   └── Services/
│   │       ├── IStudentService.cs
│   │       ├── IMenuSettingService.cs
│   │       └── ISchoolSettingsService.cs
│   │
│   ├── Common/
│   │   ├── Errors/
│   │   │   ├── Error.cs                  # sealed record(Code, Message, ErrorType)
│   │   │   └── ErrorType.cs              # NotFound | Validation | Conflict | ...
│   │   └── Result/
│   │       ├── Result.cs                 # Success() / Failure(error)
│   │       └── Result{T}.cs              # Result<T> with Value
│   │
│   ├── DTOs/
│   │   ├── Student/                      # CreateStudentDto, StudentDto, UpdateStudentDto…
│   │   ├── Document/                     # UploadDocumentDto, DocumentDto, GeneratedDocumentResult…
│   │   ├── Report/                       # StudentReportQueryDto, FeeReportDto, DashboardSummaryDto…
│   │   ├── MenuSetting/
│   │   └── Settings/                     # SchoolSettingsDto, UpdateSchoolSettingsDto…
│   │
│   ├── Mapper/
│   │   └── StudentMapper.cs              # Static ToDto() / FromDto() helpers
│   │
│   ├── Pagination/
│   │   ├── PaginationParams.cs           # PageNumber, PageSize
│   │   └── PagedList{T}.cs               # Items + TotalCount + metadata
│   │
│   ├── Services/
│   │   ├── StudentService.cs             # Implements IStudentService
│   │   └── MenuSettingService.cs
│   │
│   ├── Specifications/
│   │   ├── BaseSpecification{T}.cs       # Criteria, Includes, OrderBy, Paging
│   │   └── Students/
│   │       ├── StudentByIdSpecification.cs
│   │       ├── StudentFilterSpecification.cs
│   │       ├── StudentCountSpecification.cs
│   │       └── StudentReportSpecification.cs
│   │
│   ├── Validators/
│   │   └── Student/
│   │       └── CreateStudentValidator.cs # FluentValidation
│   │
│   └── DependencyInjection/
│       └── ApplicationServiceRegistration.cs
│
├── Project.Infrastructure/
│   ├── Persistence/
│   │   ├── AppDbContext.cs               # IdentityDbContext<ApplicationUser>
│   │   └── Configurations/               # IEntityTypeConfiguration<T> per aggregate
│   │       ├── StudentConfiguration.cs
│   │       ├── StaffConfiguration.cs
│   │       ├── FeeConfiguration.cs
│   │       ├── SalaryConfiguration.cs
│   │       ├── StaffAttendanceConfiguration.cs
│   │       ├── StaffLeaveConfiguration.cs
│   │       ├── ExpenseConfiguration.cs
│   │       ├── MenuSettingConfiguration.cs
│   │       └── SchoolSettingsConfiguration.cs
│   │
│   ├── Repositories/
│   │   ├── GenericRepository{T}.cs       # Implements IRepository<T>
│   │   └── MenuSettingRepository.cs      # Implements IMenuSettingRepository
│   │
│   ├── UnitOfWork/
│   │   └── UnitOfWork.cs                 # Wraps AppDbContext.SaveChangesAsync
│   │
│   ├── SpecificationEvaluator/
│   │   └── SpecificationEvaluator{T}.cs  # Translates BaseSpecification → IQueryable
│   │
│   ├── Interceptors/
│   │   ├── AuditInterceptor.cs           # Auto-fills CreatedAt/By, UpdatedAt/By
│   │   └── SoftDeleteInterceptor.cs      # Converts Remove → IsDeleted = true
│   │
│   ├── Identity/
│   │   ├── ApplicationUser.cs            # Extends IdentityUser
│   │   ├── IAuthService.cs
│   │   ├── AuthService.cs                # Login, Register, Invite, SetPassword, Refresh
│   │   ├── IJwtTokenGenerator.cs
│   │   ├── JwtTokenGenerator.cs
│   │   └── CurrentUserService.cs         # ICurrentUserService impl via IHttpContextAccessor
│   │
│   ├── Services/
│   │   ├── IndianDateTimeProvider.cs     # IDateTimeProvider — IST timezone
│   │   ├── EmailService.cs               # SMTP via MailKit
│   │   ├── LocalFileStorageService.cs    # IFileStorageService — wwwroot/uploads
│   │   ├── QuestPdfGeneratorService.cs   # Bonafide, Leaving, Profile PDFs
│   │   ├── SchoolSettingsService.cs      # ISchoolSettingsService
│   │   └── StudentExportService.cs       # IStudentExportService (Excel/CSV)
│   │
│   ├── Options/
│   │   ├── JwtOptions.cs                 # Issuer, Audience, SecretKey, ExpiryMinutes
│   │   ├── EmailOptions.cs               # SMTP host/port/credentials
│   │   ├── FileStorageOptions.cs         # Upload root path
│   │   └── FrontendOptions.cs            # BaseUrl for email links
│   │
│   ├── DbInitializers/
│   │   ├── IDbInitializer.cs
│   │   └── DbInitializer.cs              # Migrate + Seed on startup
│   │
│   ├── Seeding/
│   │   ├── RoleSeeder.cs
│   │   ├── AdminSeeder.cs
│   │   └── MenuSettingSeeder.cs
│   │
│   ├── Migrations/                       # EF Core auto-generated
│   │
│   └── DependencyInjection/
│       └── InfrastructureServiceRegistration.cs
│
└── Project.API/
    ├── Controllers/
    │   ├── AuthController.cs             # Login, Register, Invite, SetPassword, Refresh
    │   └── ReportsController.cs          # Dashboard, Student, Fee, Leave reports
    │
    ├── CustomResults/
    │   ├── ApiResponse{T}.cs             # { IsSuccess, Message, Data, Errors }
    │   └── PaginatedResponse.cs
    │
    ├── Handlers/
    │   └── GlobalExceptionHandler.cs     # IExceptionHandler — maps exceptions → ProblemDetails
    │
    ├── Extensions/
    │   ├── ApplicationBuilderExtensions.cs   # UseDbInitializerAsync, UseSwaggerConfig, UseSecurityHeaders
    │   └── ServiceCollectionExtensions.cs    # AddCors, AddStaticFiles, etc.
    │       SwaggerExtensions.cs
    │
    ├── Versioning/
    │   └── ApiVersioningConfig.cs
    │
    ├── Logging/
    │   └── SerilogConfig.cs              # File + Console sinks
    │
    ├── DependencyInjection/
    │   └── PresentationServiceRegistration.cs
    │
    └── Program.cs
```

---

## Key Patterns

### 1 — Aggregate Root with Factory

```csharp
// Domain/Aggregates/StudentAggregate/Student.cs
public sealed class Student : AggregateRoot
{
    private Student() { }                  // EF private constructor

    public static Student Create(...)       // validates invariants, returns instance
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("...");
        return new Student { FirstName = firstName, ... };
    }

    public void Update(...) { ... }        // behaviour on the aggregate
}
```

### 2 — Value Objects

```csharp
// Domain/ValueObjects/Email.cs
public sealed class Email
{
    public string Value { get; }
    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (!IsValid(value)) throw new ArgumentException("Invalid email.");
        return new Email(value);
    }
}
```

### 3 — Result / Error

```csharp
// In service
return Result<StudentDto>.Failure(Error.NotFound("Student.NotFound", $"Student {id} not found."));
return Result<StudentDto>.Success(StudentMapper.ToDto(student));

// In controller
var result = await _studentService.GetByIdAsync(id, ct);
if (result.IsFailure)
    return result.Error.Type switch
    {
        ErrorType.NotFound => NotFound(ApiResponse.Failure(result.Error.Message)),
        _ => BadRequest(ApiResponse.Failure(result.Error.Message))
    };
return Ok(ApiResponse<StudentDto>.Success(result.Value));
```

### 4 — Specification

```csharp
// Application/Specifications/Students/StudentFilterSpecification.cs
public class StudentFilterSpecification : BaseSpecification<Student>
{
    public StudentFilterSpecification(StudentFilterDto filter)
    {
        AddCriteria(s => !s.IsDeleted &&
            (string.IsNullOrEmpty(filter.ClassName) || s.ClassName == filter.ClassName));
        AddInclude(s => s.Documents);
        AddOrderBy(s => s.LastName);
        EnablePaging((filter.PageNumber - 1) * filter.PageSize, filter.PageSize);
    }
}
```

### 5 — Generic Repository

```csharp
// Consumed in Application
var spec = new StudentByIdSpecification(id);
var student = await _repository.FirstOrDefaultAsync(spec, ct);
```

### 6 — API Response Envelope

```csharp
// All endpoints return ApiResponse<T>
return Ok(ApiResponse<StudentDto>.Success(dto, "Student retrieved."));
return NotFound(ApiResponse.Failure("Student not found."));
```

---

## Adding a New Feature Checklist

```
[ ] Domain — create Aggregate in Aggregates/<Name>Aggregate/<Name>.cs
[ ] Domain — add Enums, Constants, ValueObjects if needed
[ ] Application — DTOs in DTOs/<Name>/
[ ] Application — Service interface in Abstractions/Services/I<Name>Service.cs
[ ] Application — Specification(s) in Specifications/<Name>/
[ ] Application — Validator in Validators/<Name>/
[ ] Application — Service implementation in Services/<Name>Service.cs
[ ] Application — Register service in ApplicationServiceRegistration.cs
[ ] Infrastructure — EF config in Persistence/Configurations/<Name>Configuration.cs
[ ] Infrastructure — DbSet<> in AppDbContext.cs
[ ] Infrastructure — Register in InfrastructureServiceRegistration.cs
[ ] Infrastructure — dotnet ef migrations add Add<Name>
[ ] API — Controller in Controllers/<Name>Controller.cs
```

---

## Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=CrsJourney;User=...;Password=...;"
  },
  "Jwt": {
    "Issuer": "CrsJourney",
    "Audience": "CrsJourneyClient",
    "SecretKey": "<min-32-char-secret>",
    "ExpiryMinutes": 60
  },
  "Email": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "...",
    "Password": "...",
    "From": "noreply@crsjourney.com"
  },
  "FileStorage": {
    "UploadRootPath": "wwwroot/uploads"
  },
  "Frontend": {
    "BaseUrl": "https://localhost:4200"
  }
}
```

---

## Running Locally

```bash
# 1. Restore packages
dotnet restore

# 2. Apply migrations (auto-runs on startup too)
dotnet ef database update --project Project.Infrastructure --startup-project Project.API

# 3. Run
dotnet run --project Project.API
```

Swagger UI: `https://localhost:{port}/swagger`
