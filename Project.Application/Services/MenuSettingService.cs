using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Application.Common.Errors;
using Project.Application.Common.Result;
using Project.Application.DTOs.MenuSetting;
using Project.Domain.Aggregates.MenuSettingAggregate;
using Project.Domain.Constants;

namespace Project.Application.Services
{
    public sealed class MenuSettingService : IMenuSettingService
    {
        private static readonly string[] AllRoles =
            [RoleConstants.Admin, RoleConstants.Teacher, RoleConstants.Accountant, RoleConstants.Staff];

        private readonly IMenuSettingRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MenuSettingService(IMenuSettingRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<MenuMatrixDto>>> GetAllAsync(CancellationToken ct = default)
        {
            var settings = await _repository.GetAllAsync(ct);

            var grouped = settings
                .GroupBy(s => s.MenuKey)
                .Select(g => new MenuMatrixDto(
                    g.Key,
                    AllRoles.ToDictionary(r => r, r => g.FirstOrDefault(s => s.Role == r)?.IsVisible ?? false)))
                .ToList();

            return Result<List<MenuMatrixDto>>.Success(grouped);
        }

        public async Task<Result<List<string>>> GetVisibleKeysForRoleAsync(string role, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(role))
                return Result<List<string>>.Failure(Error.Validation("MenuSetting.RoleRequired", "Role is required."));

            var settings = await _repository.GetByRoleAsync(role, ct);
            var keys = settings.Where(s => s.IsVisible).Select(s => s.MenuKey).ToList();

            return Result<List<string>>.Success(keys);
        }

        public async Task<Result> UpdateAsync(UpdateMenuSettingsDto dto, CancellationToken ct = default)
        {
            if (dto.Settings is null || dto.Settings.Count == 0)
                return Result.Failure(Error.Validation("MenuSetting.Empty", "No settings provided."));

            foreach (var item in dto.Settings)
            {
                var existing = await _repository.GetAsync(item.MenuKey, item.Role, ct);
                if (existing is null)
                {
                    var setting = MenuSetting.Create(item.MenuKey, item.Role, item.IsVisible);
                    await _repository.AddAsync(setting, ct);
                }
                else
                {
                    existing.SetVisibility(item.IsVisible);
                    _repository.Update(existing);
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }
    }
}
