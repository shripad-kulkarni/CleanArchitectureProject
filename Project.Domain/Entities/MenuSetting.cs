using Project.Domain.Primitives;

namespace Project.Domain.Entities
{
    public sealed class MenuSetting : Entity
    {
        private MenuSetting() { }

        public MenuSetting(
            string menuKey,
            string label,
            string? icon,
            string? parentKey,
            int sortOrder,
            string role,
            bool isVisible)
        {
            MenuKey   = menuKey;
            Label     = label;
            Icon      = icon;
            ParentKey = parentKey;
            SortOrder = sortOrder;
            Role      = role;
            IsVisible = isVisible;
        }

        public string MenuKey { get; private set; } = string.Empty;
        public string Label { get; private set; } = string.Empty;
        public string? Icon { get; private set; }
        public string? ParentKey { get; private set; }
        public int SortOrder { get; private set; }
        public string Role { get; private set; } = string.Empty;
        public bool IsVisible { get; private set; }

        public void SetVisibility(bool isVisible) => IsVisible = isVisible;
    }
}
