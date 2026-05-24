using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Identity;
using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Infrastructure.Repositories;
using Project.Application.DTOs.Expense;
using Project.Application.DTOs.Fee;
using Project.Application.DTOs.Salary;
using Project.Application.DTOs.Staff;
using Project.Application.DTOs.Student;
using Project.Application.Services;
using Project.Application.Validators.Expense;
using Project.Application.Validators.Fee;
using Project.Application.Validators.Salary;
using Project.Application.Validators.Staff;
using Project.Application.Validators.Student;
using Project.Infrastructure.DbInitializers;
using Project.Infrastructure.Identity;
using Project.Infrastructure.Interceptors;
using Project.Infrastructure.Options;
using Project.Infrastructure.Persistence;
using Project.Infrastructure.Repositories;
using QuestPDF.Infrastructure;
using Project.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // Options
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
            services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));
            services.Configure<FrontendOptions>(configuration.GetSection(FrontendOptions.SectionName));

            // Interceptors
            services.AddScoped<AuditInterceptor>();
            services.AddScoped<SoftDeleteInterceptor>();

            // DbContext
            // DbContext — MySQL via Pomelo
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });

            // Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // JWT Authentication
            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

            // Generic Repository & UnitOfWork
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            // Identity Services
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // External Services
            services.AddScoped<IDateTimeProvider, IndianDateTimeProvider>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IPdfGeneratorService, QuestPdfGeneratorService>();
            services.AddSingleton<IStudentExportService, StudentExportService>();
            services.AddSingleton<IExpenseExportService, ExpenseExportService>();
            services.AddSingleton<ILeaveExportService, LeaveExportService>();
            services.AddScoped<IDbInitializer, DbInitializer>();

            // Validators
            services.AddScoped<IValidator<CreateStudentDto>, CreateStudentValidator>();
            services.AddScoped<IValidator<CreateStaffDto>, CreateStaffValidator>();
            services.AddScoped<IValidator<CreateFeeDto>, CreateFeeValidator>();
            services.AddScoped<IValidator<CollectFeeDto>, CollectFeeValidator>();
            services.AddScoped<IValidator<CreateSalaryDto>, CreateSalaryValidator>();
            services.AddScoped<IValidator<CreateExpenseDto>, CreateExpenseValidator>();

            // Application services
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IFeeService, FeeService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<ILeaveService, LeaveService>();
            services.AddScoped<ISalaryService, SalaryService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IMenuSettingRepository, MenuSettingRepository>();
            services.AddScoped<IMenuSettingService, MenuSettingService>();
            services.AddScoped<ISchoolSettingsService, SchoolSettingsService>();

            return services;
        }
    }
}
