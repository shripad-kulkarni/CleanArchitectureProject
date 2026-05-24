namespace Project.Domain.Aggregates.MenuSettingAggregate
{
    public sealed class MenuSetting
    {
        public int Id { get; private set; }
        public string MenuKey { get; private set; } = string.Empty;
        public string Role { get; private set; } = string.Empty;
        public bool IsVisible { get; private set; }

        private MenuSetting() { }

        public static MenuSetting Create(string menuKey, string role, bool isVisible)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(menuKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(role);

            return new MenuSetting
            {
                MenuKey = menuKey,
                Role = role,
                IsVisible = isVisible
            };
        }

        public void SetVisibility(bool isVisible) => IsVisible = isVisible;
    }
}
