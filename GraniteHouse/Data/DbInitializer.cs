using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Models;
using GraniteHouse.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async void Initialize()
        {
            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }

            if (_db.Roles.Any(r=>r.Name == StaticDetails.SuperAdminEndUser)) return;

            _roleManager.CreateAsync(new IdentityRole(StaticDetails.AdminEndUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.SuperAdminEndUser)).GetAwaiter().GetResult();

            

            _userManager.CreateAsync(new ApplicationUser()
            {
                UserName = "admin@gmail.com",
                Name = "Arfizur Rahman",
                Email = "admin@gmail.com",
                PhoneNumber = "4545454545",
                EmailConfirmed = true
            }, "Admin123*").GetAwaiter().GetResult();

            IdentityUser user = await _db.Users.Where(e => e.Email == "admin@gmail.com").FirstOrDefaultAsync();
            await _userManager.AddToRoleAsync(user, StaticDetails.SuperAdminEndUser);


            //var userAdmin = new ApplicationUser
            //{
            //    UserName = "admin@gmail.com",
            //    Email = "admin@gmail.com",
            //    PhoneNumber = "4545454545",
            //    Name = "Arfizur Rahman"
            //};

            //var resultAdmin = await _userManager.CreateAsync(userAdmin, "Admin123*");
            //await _userManager.AddToRoleAsync(userAdmin, StaticDetails.SuperAdminEndUser);
        }

    }
}
