using Microsoft.EntityFrameworkCore;
using Project.Domain.Constants;
using Project.Domain.Entities;
using Project.Infrastructure.Persistence;

namespace Project.Infrastructure.Persistence.Seeding
{
    public static class MenuSettingSeeder
    {
        private record MenuDef(
            string Key,
            string Label,
            string? Icon,
            string? ParentKey,
            int SortOrder,
            string[] VisibleRoles);

        private static readonly MenuDef[] Menus =
        [
            new("dashboard",         "Dashboard",    "layout-dashboard", null,          1, [RoleConstants.Admin, RoleConstants.Manager, RoleConstants.User, RoleConstants.Guest]),

            new("users",             "Users",        "users",            null,          2, [RoleConstants.Admin]),

            new("records",           "Records",      "folder",           null,          3, [RoleConstants.Admin, RoleConstants.Manager]),
            new("records.list",      "All Records",  "list",             "records",     1, [RoleConstants.Admin, RoleConstants.Manager]),
            new("records.import",    "Import",       "upload",           "records",     2, [RoleConstants.Admin]),

            new("finances",          "Finances",     "wallet",           null,          4, [RoleConstants.Admin, RoleConstants.Manager]),
            new("finances.fees",     "Fees",         "receipt",          "finances",    1, [RoleConstants.Admin, RoleConstants.Manager]),
            new("finances.salary",   "Salary",       "banknote",         "finances",    2, [RoleConstants.Admin]),

            new("documents",         "Documents",    "file-text",        null,          5, [RoleConstants.Admin, RoleConstants.Manager, RoleConstants.User]),

            new("reports",           "Reports",      "bar-chart-2",      null,          6, [RoleConstants.Admin, RoleConstants.Manager]),
            new("reports.summary",   "Summary",      "pie-chart",        "reports",     1, [RoleConstants.Admin, RoleConstants.Manager]),
            new("reports.exports",   "Exports",      "download",         "reports",     2, [RoleConstants.Admin]),

            new("settings",          "Settings",     "settings",         null,          7, [RoleConstants.Admin]),
        ];

        public static async Task SeedAsync(AppDbContext db)
        {
            var any = await db.MenuSettings.AnyAsync();
            if (any) return;

            var settings = new List<MenuSetting>();

            foreach (var menu in Menus)
            {
                foreach (var role in RoleConstants.AllRoles)
                {
                    settings.Add(new MenuSetting(
                        menu.Key,
                        menu.Label,
                        menu.Icon,
                        menu.ParentKey,
                        menu.SortOrder,
                        role,
                        menu.VisibleRoles.Contains(role)));
                }
            }

            await db.MenuSettings.AddRangeAsync(settings);
            await db.SaveChangesAsync();
        }
    }
}
