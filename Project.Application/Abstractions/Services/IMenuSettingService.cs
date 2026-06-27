using Project.Application.Common.Result;
using Project.Application.DTOs.MenuSetting;

namespace Project.Application.Abstractions.Services
{
    public interface IMenuSettingService
    {
        Task<Result<List<MenuNodeDto>>> GetAllAsync(CancellationToken ct = default);
        Task<Result<List<MenuNodeDto>>> GetMenuForRoleAsync(string role, CancellationToken ct = default);
        Task<Result> UpdateAsync(UpdateMenuSettingsDto dto, CancellationToken ct = default);
    }
}
