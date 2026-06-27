using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Application.Common.Errors;
using Project.Application.Common.Result;
using Project.Application.DTOs.MenuSetting;
using Project.Domain.Constants;

namespace Project.Application.Services
{
    public sealed class MenuSettingService : IMenuSettingService
    {
        private readonly IMenuSettingRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MenuSettingService(IMenuSettingRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<MenuNodeDto>>> GetAllAsync(CancellationToken ct = default)
        {
            var settings = await _repository.GetAllAsync(ct);
            var tree = BuildTree(settings, visibleForRole: null);
            return Result<List<MenuNodeDto>>.Success(tree);
        }

        public async Task<Result<List<MenuNodeDto>>> GetMenuForRoleAsync(string role, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(role))
                return Result<List<MenuNodeDto>>.Failure(Error.Validation("MenuSetting.RoleRequired", "Role is required."));

            var settings = await _repository.GetByRoleAsync(role, ct);
            var tree = BuildTree(settings, visibleForRole: role);
            return Result<List<MenuNodeDto>>.Success(tree);
        }

        public async Task<Result> UpdateAsync(UpdateMenuSettingsDto dto, CancellationToken ct = default)
        {
            if (dto.Settings is null || dto.Settings.Count == 0)
                return Result.Failure(Error.Validation("MenuSetting.Empty", "No settings provided."));

            foreach (var item in dto.Settings)
            {
                var existing = await _repository.GetAsync(item.MenuKey, item.Role, ct);
                if (existing is null) continue;

                existing.SetVisibility(item.IsVisible);
                _repository.Update(existing);
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }

        private static List<MenuNodeDto> BuildTree(
            IEnumerable<Domain.Entities.MenuSetting> settings,
            string? visibleForRole)
        {
            var grouped = settings
                .GroupBy(s => s.MenuKey)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        First = g.First(),
                        RoleVisibility = RoleConstants.AllRoles.ToDictionary(
                            r => r,
                            r => g.FirstOrDefault(s => s.Role == r)?.IsVisible ?? false)
                    });

            var nodes = grouped
                .OrderBy(kv => kv.Value.First.SortOrder)
                .ToDictionary(
                    kv => kv.Key,
                    kv => new MenuNodeDto(
                        kv.Key,
                        kv.Value.First.Label,
                        kv.Value.First.Icon,
                        kv.Value.First.SortOrder,
                        kv.Value.RoleVisibility,
                        new List<MenuNodeDto>()));

            var roots = new List<MenuNodeDto>();

            foreach (var (key, data) in grouped.OrderBy(kv => kv.Value.First.SortOrder))
            {
                var node = nodes[key];

                if (visibleForRole != null && !data.RoleVisibility.GetValueOrDefault(visibleForRole))
                    continue;

                var parentKey = data.First.ParentKey;
                if (parentKey != null && nodes.TryGetValue(parentKey, out var parentNode))
                    parentNode.Children.Add(node);
                else
                    roots.Add(node);
            }

            return roots;
        }
    }
}
