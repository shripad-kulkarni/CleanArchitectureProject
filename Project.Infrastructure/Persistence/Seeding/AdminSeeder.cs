using Microsoft.AspNetCore.Identity;
using Project.Domain.Aggregates;
using Project.Domain.Constants;
using Project.Infrastructure.Identity;

namespace Project.Infrastructure.Persistence.Seeding
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@gmail.com";
            const string adminPassword = "Admin@123";

            var existing = await userManager.FindByEmailAsync(adminEmail);
            if (existing is not null) return;

            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Super",
                LastName = "Admin",
                IsActive = true,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, adminPassword);
            await userManager.AddToRoleAsync(admin, RoleConstants.Admin);
        }
    }
}
