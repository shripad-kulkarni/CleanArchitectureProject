using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Aggregates;
using Project.Infrastructure.Identity;
using Project.Infrastructure.Persistence.Seeding;

namespace Project.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            AppDbContext db,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            if (db.Database.GetPendingMigrations().Any())
            {
                await db.Database.MigrateAsync();
            }
               
            await RoleSeeder.SeedAsync(roleManager);
            await AdminSeeder.SeedAsync(userManager);
            await MenuSettingSeeder.SeedAsync(db);
        }
    }
}
