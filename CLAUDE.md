# CrsJourney — Clean Architecture API

## Tech Stack
- **Runtime**: .NET 8, ASP.NET Core Web API
- **ORM**: Entity Framework Core 8 + Pomelo MySQL provider
- **Identity**: ASP.NET Core Identity + JWT Bearer
- **Validation**: FluentValidation
- **PDF Generation**: QuestPDF
- **Logging**: Serilog
- **API Versioning**: Asp.Versioning

---

## Solution Layout

```
CleanArchitectureProject.sln
├── Project.Domain           ← innermost — no dependencies
├── Project.Application      ← depends on Domain only
├── Project.Infrastructure   ← depends on Domain + Application
└── Project.API              ← depends on all three layers
```

---

## Layer Responsibilities

### Project.Domain
Pure business model — zero framework dependencies.

| Folder | Contents |
|--------|----------|
| `Primitives/` | `Entity` (base with audit + soft-delete fields), `AggregateRoot` |
| `Aggregates/` | One sub-folder per aggregate root (e.g. `StudentAggregate/`) |
| `ValueObjects/` | `Email`, `PhoneNumber`, `Address`, `Money`, `DateRange` — immutable, `Create()` factory |
| `Enums/` | `Gender`, `DocumentType`, `FeeStatus`, `LeaveStatus`, `LeaveType`, `SalaryStatus`, `StaffRole`, `AttendanceStatus` |
| `Constants/` | `DocumentConstants`, `FeeConstants`, `RoleConstants` |
| `Exceptions/` | `DomainException` and domain-specific exceptions |

**Entity base class** (`Primitives/Entity.cs`):
```csharp
public abstract class Entity : IEquatable<Entity>
{
    public int Id { get; private set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }   // soft delete
}
```

**Aggregate pattern**: Each aggregate root is a `sealed class` extending `AggregateRoot`. All constructors are `private`. Creation uses a static `Create(...)` factory that validates invariants and returns the new instance.

---

### Project.Application
Use-case orchestration — depends only on Domain.

| Folder | Contents |
|--------|----------|
| `Abstractions/Persistence/` | `IRepository<T>`, `IUnitOfWork`, `IMenuSettingRepository` |
| `Abstractions/Identity/` | `ICurrentUserService` |
| `Abstractions/ExternalServices/` | `IEmailService`, `IFileStorageService`, `IPdfGeneratorService`, `IStudentExportService`, `IDateTimeProvider` |
| `Abstractions/Services/` | Service contracts consumed by Controllers |
| `Common/Errors/` | `Error` (sealed record), `ErrorType` enum |
| `Common/Result/` | `Result`, `Result<T>` |
| `DTOs/` | Request/response DTOs grouped by feature (Student, Staff, Fee…) |
| `Mapper/` | Static mapper classes (e.g. `StudentMapper`) |
| `Pagination/` | `PaginationParams`, `PagedList<T>` |
| `Services/` | Concrete service implementations (`StudentService`, `MenuSettingService`) |
| `Specifications/` | `BaseSpecification<T>` + concrete specs per entity |
| `Validators/` | FluentValidation validators grouped by feature |
| `DependencyInjection/` | `ApplicationServiceRegistration` extension method |

**Result pattern** — every service method returns `Result` or `Result<T>`:
```csharp
// Success
return Result<StudentDto>.Success(dto);

// Failure
return Result<StudentDto>.Failure(Error.NotFound("Student.NotFound", $"..."));
```

**Error factory methods**: `Error.NotFound`, `Error.Validation`, `Error.Conflict`, `Error.Failure`, `Error.Unauthorized`.

**Specification pattern** — all queries go through `BaseSpecification<T>` subclasses:
```csharp
protected void AddCriteria(Expression<Func<T, bool>> criteria);
protected void AddInclude<TProperty>(Expression<Func<T, TProperty>> includeExpression);
protected void AddOrderBy / AddOrderByDescending(...)
protected void EnablePaging(int skip, int take);
```

---

### Project.Infrastructure
Framework & persistence concerns — depends on Domain + Application.

| Folder | Contents |
|--------|----------|
| `Persistence/` | `AppDbContext` (IdentityDbContext), EF `Configurations/` |
| `Repositories/` | `GenericRepository<T>`, `MenuSettingRepository` |
| `UnitOfWork/` | `UnitOfWork` wrapping `AppDbContext.SaveChangesAsync` |
| `Identity/` | `ApplicationUser`, `AuthService`, `JwtTokenGenerator`, `CurrentUserService`, interfaces |
| `Interceptors/` | `AuditInterceptor` (auto-stamps Created/Updated), `SoftDeleteInterceptor` |
| `Services/` | `EmailService`, `LocalFileStorageService`, `QuestPdfGeneratorService`, `IndianDateTimeProvider`, `SchoolSettingsService`, `StudentExportService` |
| `Options/` | Configuration POCOs: `JwtOptions`, `EmailOptions`, `FileStorageOptions`, `FrontendOptions` |
| `DbInitializers/` | `IDbInitializer`, `DbInitializer` (runs migrations + seeding on startup) |
| `Seeding/` | `AdminSeeder`, `RoleSeeder`, `MenuSettingSeeder` |
| `SpecificationEvaluator/` | `SpecificationEvaluator<T>` translates specs to EF queries |
| `Migrations/` | EF Core migration files |
| `DependencyInjection/` | `InfrastructureServiceRegistration` extension method |

**Interceptors** hook into EF Core's `SaveChangesInterceptor`:
- `AuditInterceptor` — sets `CreatedAt/By` on `Added` and `UpdatedAt/By` on `Modified`
- `SoftDeleteInterceptor` — converts `Remove` to setting `IsDeleted = true`

**Generic Repository** exposes:
```csharp
GetByIdAsync / FirstOrDefaultAsync / ListAsync / CountAsync
ExistsAsync / ExistsIgnoringFiltersAsync
AddAsync / Update / Delete
```

---

### Project.API
HTTP presentation layer — depends on all layers.

| Folder | Contents |
|--------|----------|
| `Controllers/` | `AuthController`, `ReportsController`, etc. |
| `CustomResults/` | `ApiResponse`, `ApiResponse<T>` (uniform envelope) |
| `DependencyInjection/` | `PresentationServiceRegistration` |
| `Extensions/` | `ApplicationBuilderExtensions`, `ServiceCollectionExtensions`, `SwaggerExtensions` |
| `Handlers/` | `GlobalExceptionHandler` (IExceptionHandler) |
| `Logging/` | `SerilogConfig` |
| `Versioning/` | `ApiVersioningConfig` |
| `Properties/` | `launchSettings.json` |
| `Program.cs` | Startup wiring |

**Controller convention**: versioned routes `api/v{version:apiVersion}/resource`, return `ApiResponse<T>` wrapping the service `Result<T>`.

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/students")]
public sealed class StudentsController : ControllerBase { ... }
```

**Error → HTTP mapping** (in controllers or GlobalExceptionHandler):
| ErrorType | HTTP Status |
|-----------|-------------|
| NotFound | 404 |
| Validation | 400 |
| Conflict | 409 |
| Unauthorized | 401 |
| Failure | 500 |

---

## How to Add a New Feature (e.g. "Attendance")

1. **Domain** — Add `AttendanceAggregate/Attendance.cs` extending `AggregateRoot`
2. **Application**
   - Add DTO(s) in `DTOs/Attendance/`
   - Add `Abstractions/Services/IAttendanceService.cs`
   - Add `Specifications/Attendance/...Specification.cs`
   - Add validator in `Validators/Attendance/`
   - Implement `Services/AttendanceService.cs`
   - Register in `ApplicationServiceRegistration.cs`
3. **Infrastructure**
   - Add EF config in `Persistence/Configurations/AttendanceConfiguration.cs`
   - Add `DbSet<Attendance>` in `AppDbContext.cs`
   - Register in `InfrastructureServiceRegistration.cs`
   - Run `dotnet ef migrations add AddAttendance`
4. **API**
   - Add `Controllers/AttendanceController.cs`

---

## Dependency Injection Startup Order

```csharp
// Program.cs
builder.Services
    .AddInfrastructureServices(config)   // DB, Identity, JWT, Repos, External services
    .AddApplicationServices(config)      // Services, Validators, MediatR (if added)
    .AddPresentationServices(config);    // Cors, Swagger, Versioning, ExceptionHandler
```

---

## Database

- **Provider**: MySQL via Pomelo (`Pomelo.EntityFrameworkCore.MySql`)
- **Connection string key**: `"DefaultConnection"` in `appsettings.json`
- **Migrations assembly**: `Project.Infrastructure`
- **Global query filters**: Soft-delete filter (`IsDeleted == false`) applied in EF configurations
- **Auto-migration + seeding**: runs via `app.UseDbInitializerAsync()` on startup

---

## Key Patterns Summary

| Pattern | Where |
|---------|-------|
| Aggregate Root + Factory | `Project.Domain/Aggregates/` |
| Value Objects | `Project.Domain/ValueObjects/` |
| Result / Error | `Project.Application/Common/` |
| Repository + UoW | Application abstractions, Infrastructure impl |
| Specification | Application specifications, Infrastructure evaluator |
| Soft Delete via Interceptor | `Project.Infrastructure/Interceptors/` |
| Audit via Interceptor | `Project.Infrastructure/Interceptors/` |
| Options pattern | `Project.Infrastructure/Options/` |
| API versioning | `Project.API/Versioning/` |
| Global exception handling | `Project.API/Handlers/` |
| Uniform API envelope | `Project.API/CustomResults/ApiResponse<T>` |
