using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Aggregates.ExpenseAggregate;
using Project.Domain.Aggregates.FeeAggregate;
using Project.Domain.Aggregates.MenuSettingAggregate;
using Project.Domain.Aggregates.SchoolSettingAggregate;
using Project.Domain.Aggregates.SalaryAggregate;
using Project.Domain.Aggregates.StaffAggregate;
using Project.Domain.Aggregates.StaffAttendanceAggregates;
using Project.Domain.Aggregates.StaffLeaveAggregate;
using Project.Domain.Aggregates.StudentAggregate;
using Project.Infrastructure.Identity;
using Project.Infrastructure.Interceptors;

namespace Project.Infrastructure.Persistence
{
    public sealed class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly AuditInterceptor _auditInterceptor;
        private readonly SoftDeleteInterceptor _softDeleteInterceptor;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            AuditInterceptor auditInterceptor,
            SoftDeleteInterceptor softDeleteInterceptor)
            : base(options)
        {
            _auditInterceptor = auditInterceptor;
            _softDeleteInterceptor = softDeleteInterceptor;
        }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();

        public DbSet<Staff> Staffs => Set<Staff>();
        public DbSet<StaffAttendance> StaffAttendances => Set<StaffAttendance>();
        public DbSet<StaffLeave> StaffLeaves => Set<StaffLeave>();

        public DbSet<Fee> Fees => Set<Fee>();
        public DbSet<FeeInstallment> FeeInstallments => Set<FeeInstallment>();

        public DbSet<Salary> Salaries => Set<Salary>();
        public DbSet<SalaryIncrement> SalaryIncrements => Set<SalaryIncrement>();

        public DbSet<Expense> Expenses => Set<Expense>();

        public DbSet<MenuSetting> MenuSettings => Set<MenuSetting>();
        public DbSet<SchoolSettings> SchoolSettings => Set<SchoolSettings>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor, _softDeleteInterceptor);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Auto-apply all IEntityTypeConfiguration<T> from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
