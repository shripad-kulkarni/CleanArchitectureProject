using Microsoft.EntityFrameworkCore;
using Project.Application.Abstractions.Persistence;
using Project.Domain.Aggregates.MenuSettingAggregate;
using Project.Infrastructure.Persistence;

namespace Project.Infrastructure.Repositories
{
    public sealed class MenuSettingRepository : IMenuSettingRepository
    {
        private readonly AppDbContext _db;

        public MenuSettingRepository(AppDbContext db) => _db = db;

        public Task<List<MenuSetting>> GetAllAsync(CancellationToken ct = default) =>
            _db.MenuSettings.ToListAsync(ct);

        public Task<List<MenuSetting>> GetByRoleAsync(string role, CancellationToken ct = default) =>
            _db.MenuSettings.Where(m => m.Role == role).ToListAsync(ct);

        public Task<MenuSetting?> GetAsync(string menuKey, string role, CancellationToken ct = default) =>
            _db.MenuSettings.FirstOrDefaultAsync(m => m.MenuKey == menuKey && m.Role == role, ct);

        public Task AddAsync(MenuSetting setting, CancellationToken ct = default) =>
            _db.MenuSettings.AddAsync(setting, ct).AsTask();

        public void Update(MenuSetting setting) => _db.MenuSettings.Update(setting);
    }
}
