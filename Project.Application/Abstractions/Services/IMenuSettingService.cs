using Project.Application.Common.Result;
using Project.Application.DTOs.MenuSetting;

namespace Project.Application.Abstractions.Services
{
    public interface IMenuSettingService
    {
        Task<Result<List<MenuMatrixDto>>> GetAllAsync(CancellationToken ct = default);
        Task<Result<List<string>>> GetVisibleKeysForRoleAsync(string role, CancellationToken ct = default);
        Task<Result> UpdateAsync(UpdateMenuSettingsDto dto, CancellationToken ct = default);
    }
}
