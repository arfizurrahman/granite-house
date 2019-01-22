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
            _db.Database.Migrate(); 

            if (_db.Roles.Any(r=>r.Name == StaticDetails.SuperAdminEndUser)) return;

            _roleManager.CreateAsync(new IdentityRole(StaticDetails.AdminEndUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.SuperAdminEndUser)).GetAwaiter().GetResult();

            var user = new ApplicationUser()
            {
                UserName = "admin@gmail.com",
                Name = "Arfizur Rahman",
                Email = "admin@gmail.com",
                PhoneNumber = "4545454545",
                EmailConfirmed = true
            };

            _userManager.CreateAsync(user, "Admin123*").GetAwaiter().GetResult();
            await _userManager.AddToRoleAsync(await _userManager.FindByEmailAsync("admin@gmail.com"), StaticDetails.SuperAdminEndUser);


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
