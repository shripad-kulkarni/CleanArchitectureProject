using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Identity;
using Project.Infrastructure.Persistence;
using Project.Infrastructure.Seeding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.DbInitializers
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DbInitializer(
            AppDbContext db,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task InitializeAsync()
        {
            await _db.Database.MigrateAsync();
            await RoleSeeder.SeedAsync(_roleManager);
            await AdminSeeder.SeedAsync(_userManager);
            await MenuSettingSeeder.SeedAsync(_db);
        }
    }
}
