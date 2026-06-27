using Microsoft.EntityFrameworkCore;
using Project.Application.Abstractions.Persistence;
using Project.Domain.Entities;
using Project.Infrastructure.Persistence;

namespace Project.Infrastructure.Persistence.Repositories
{
    public sealed class MenuSettingRepository : IMenuSettingRepository
    {
        private readonly AppDbContext _db;

        public MenuSettingRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<MenuSetting>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.MenuSettings.ToListAsync(ct);
        }

        public async Task<List<MenuSetting>> GetByRoleAsync(string role, CancellationToken ct = default)
        {
            return await _db.MenuSettings.Where(m => m.Role == role).ToListAsync(ct);
        }

        public async Task<MenuSetting?> GetAsync(string menuKey, string role, CancellationToken ct = default)
        {
            return await _db.MenuSettings
                .FirstOrDefaultAsync(m => m.MenuKey == menuKey && m.Role == role, ct);
        }

        public async Task AddAsync(MenuSetting setting, CancellationToken ct = default)
        {
            await _db.MenuSettings.AddAsync(setting, ct);
        }

        public void Update(MenuSetting setting)
        {
            _db.MenuSettings.Update(setting);
        }
    }
}
