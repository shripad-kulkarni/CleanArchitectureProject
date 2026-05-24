namespace Project.Application.DTOs.MenuSetting
{
    public record MenuSettingDto(string MenuKey, string Role, bool IsVisible);

    public record MenuMatrixDto(string MenuKey, Dictionary<string, bool> RoleVisibility);

    public record UpdateMenuSettingsDto(List<MenuSettingDto> Settings);
}
