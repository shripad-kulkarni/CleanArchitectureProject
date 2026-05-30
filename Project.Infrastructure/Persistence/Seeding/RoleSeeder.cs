using Microsoft.AspNetCore.Identity;
using Project.Domain.Constants;

namespace Project.Infrastructure.Persistence.Seeding
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in RoleConstants.AllRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
