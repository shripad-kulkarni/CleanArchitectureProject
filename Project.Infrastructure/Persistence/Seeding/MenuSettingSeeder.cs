using Microsoft.EntityFrameworkCore;
using Project.Domain.Constants;
using Project.Domain.Entities;
using Project.Infrastructure.Persistence;

namespace Project.Infrastructure.Persistence.Seeding
{
    public static class MenuSettingSeeder
    {
        // menuKey → roles that can see it by default
        private static readonly Dictionary<string, string[]> Defaults = new()
        {
            ["dashboard"] = [RoleConstants.Admin, RoleConstants.Manager, RoleConstants.User, RoleConstants.Guest],
            ["records"]   = [RoleConstants.Admin, RoleConstants.Manager],
            ["finances"]  = [RoleConstants.Admin, RoleConstants.Manager],
            ["documents"] = [RoleConstants.Admin, RoleConstants.Manager, RoleConstants.User],
            ["reports"]   = [RoleConstants.Admin, RoleConstants.Manager],
            ["settings"]  = [RoleConstants.Admin],
            ["users"]     = [RoleConstants.Admin],
        };

        public static async Task SeedAsync(AppDbContext db)
        {
            var any = await db.MenuSettings.AnyAsync();
            if (any) return;

            var settings = new List<MenuSetting>();
            foreach (var (key, visibleRoles) in Defaults)
            {
                foreach (var role in RoleConstants.AllRoles)
                {
                    settings.Add(MenuSetting.Create(key, role, visibleRoles.Contains(role)));
                }
            }

            await db.MenuSettings.AddRangeAsync(settings);
            await db.SaveChangesAsync();
        }
    }
}
