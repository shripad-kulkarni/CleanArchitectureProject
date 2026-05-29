using Project.Domain.Primitives;

namespace Project.Domain.Entities
{
    public sealed class MenuSetting : AggregateRoot
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
