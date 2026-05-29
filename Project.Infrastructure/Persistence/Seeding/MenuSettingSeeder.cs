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
            ["dashboard"]  = [RoleConstants.Admin, RoleConstants.Teacher, RoleConstants.Accountant, RoleConstants.Staff],
            ["students"]   = [RoleConstants.Admin, RoleConstants.Teacher],
            ["fees"]       = [RoleConstants.Admin, RoleConstants.Accountant],
            ["attendance"] = [RoleConstants.Admin, RoleConstants.Teacher, RoleConstants.Staff],
            ["documents"]  = [RoleConstants.Admin, RoleConstants.Teacher],
            ["staff"]      = [RoleConstants.Admin],
            ["leaves"]     = [RoleConstants.Admin, RoleConstants.Teacher, RoleConstants.Staff],
            ["salaries"]   = [RoleConstants.Admin, RoleConstants.Accountant],
            ["expenses"]   = [RoleConstants.Admin, RoleConstants.Accountant],
            ["reports"]    = [RoleConstants.Admin],
            ["users"]      = [RoleConstants.Admin],
        };

        private static readonly string[] AllRoles =
            [RoleConstants.Admin, RoleConstants.Teacher, RoleConstants.Accountant, RoleConstants.Staff];

        public static async Task SeedAsync(AppDbContext db)
        {
            var any = await db.MenuSettings.AnyAsync();
            if (any) return;

            var settings = new List<MenuSetting>();
            foreach (var (key, visibleRoles) in Defaults)
            {
                foreach (var role in AllRoles)
                {
                    settings.Add(MenuSetting.Create(key, role, visibleRoles.Contains(role)));
                }
            }

            await db.MenuSettings.AddRangeAsync(settings);
            await db.SaveChangesAsync();
        }
    }
}
