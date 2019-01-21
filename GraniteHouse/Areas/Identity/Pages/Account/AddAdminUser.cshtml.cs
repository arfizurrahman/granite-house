using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Models;
using GraniteHouse.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GraniteHouse.Areas.Identity.Pages.Account
{
    public class AddAdminUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddAdminUserModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> OnGet()
        {
            //Create rules and super admin user
            if (!await _roleManager.RoleExistsAsync(StaticDetails.AdminEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.AdminEndUser));
            }

            if (!await _roleManager.RoleExistsAsync(StaticDetails.SuperAdminEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(StaticDetails.SuperAdminEndUser));

                var userAdmin = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    PhoneNumber = "4545454545",
                    Name = "Arfizur Rahman"
                };

                var resultAdmin = await _userManager.CreateAsync(userAdmin, "Admin123*");
                await _userManager.AddToRoleAsync(userAdmin, StaticDetails.SuperAdminEndUser);
            }

            

            return Page();
        }
    }
}