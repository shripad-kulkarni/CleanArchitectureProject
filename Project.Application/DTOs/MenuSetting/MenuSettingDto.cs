namespace Project.Application.DTOs.MenuSetting
{
    public record MenuNodeDto(
        string MenuKey,
        string Label,
        string? Icon,
        int SortOrder,
        Dictionary<string, bool> RoleVisibility,
        List<MenuNodeDto> Children);

    public record MenuSettingDto(string MenuKey, string Role, bool IsVisible);

    public record UpdateMenuSettingsDto(List<MenuSettingDto> Settings);
}
