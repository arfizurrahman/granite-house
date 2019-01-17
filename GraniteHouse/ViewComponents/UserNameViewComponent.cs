using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraniteHouse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdemtity = (ClaimsIdentity) this.User.Identity;
            var claims = claimsIdemtity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await _db.ApplicationUsers.Where(u => u.Id == claims.Value).FirstOrDefaultAsync();

            return View(userFromDb);
        }
    }
}
