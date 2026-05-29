using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Aggregates;
using Project.Domain.Aggregates.PaymentAggregate;
using Project.Domain.Aggregates.UserAggregate;
using Project.Domain.Entities;
using Project.Infrastructure.Identity;
using Project.Infrastructure.Persistence.Interceptors;

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

        public DbSet<User> DomainUsers => Set<User>();
        public DbSet<UserDocument> UserDocuments => Set<UserDocument>();

        public DbSet<MenuSetting> MenuSettings => Set<MenuSetting>();
        public DbSet<InfoSetting> InfoSettings => Set<InfoSetting>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor, _softDeleteInterceptor);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
