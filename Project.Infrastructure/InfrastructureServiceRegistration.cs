using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Identity;
using Project.Application.Abstractions.Persistence;
using Project.Infrastructure.Hubs;
using Project.Infrastructure.Identity;
using Project.Infrastructure.Options;
using Project.Infrastructure.Persistence;
using DinkToPdf;
using DinkToPdf.Contracts;
using QuestPDF.Infrastructure;
using Project.Infrastructure.Services;
using System.Text;
using Project.Infrastructure.Persistence.Repositories;
using Project.Infrastructure.Persistence.Interceptors;
using Project.Domain.Aggregates;

namespace Project.Infrastructure
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
            services.Configure<RazorpayOptions>(configuration.GetSection(RazorpayOptions.SectionName));

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

                // SignalR sends the JWT via query string instead of Authorization header
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(token) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                            context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

            // SignalR
            services.AddSignalR();
            services.AddSingleton<INotificationService, NotificationService>();

            // Generic Repository & UnitOfWork
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Identity Services
            
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IIdentityUserLookupService, IdentityUserLookupService>();

            // External Services
            services.AddScoped<IDateTimeProvider, IndianDateTimeProvider>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IExportService, ExportService>();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddScoped<RazorViewRenderer>();
            services.AddScoped<IUserProfileReportService, UserProfileReportService>();
            services.AddScoped<ICertificateService, CertificateService>();
 
            services.AddScoped<IMenuSettingRepository, MenuSettingRepository>();
            services.AddScoped<IPaymentGatewayService, RazorpayService>();

            return services;
        }
    }
}
