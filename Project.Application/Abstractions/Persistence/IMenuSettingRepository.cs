using Project.Domain.Entities;

namespace Project.Application.Abstractions.Persistence
{
    public interface IMenuSettingRepository
    {
        Task<List<MenuSetting>> GetAllAsync(CancellationToken ct = default);
        Task<List<MenuSetting>> GetByRoleAsync(string role, CancellationToken ct = default);
        Task<MenuSetting?> GetAsync(string menuKey, string role, CancellationToken ct = default);
        Task AddAsync(MenuSetting setting, CancellationToken ct = default);
        void Update(MenuSetting setting);
    }
}
